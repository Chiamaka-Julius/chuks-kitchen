using Chuks_Kitchen.Migrations.Data;
using Chuks_Kitchen.Models;
using Chuks_Kitchen.Services.BL.Interfaces;
using Chuks_Kitchen.Services.Contracts;
using Chuks_Kitchen.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace Chuks_Kitchen.Services.BL.Implementations;

public sealed class UserBL : IUserBL
{
    private static readonly HashSet<string> ValidReferralCodes =
    [
        "CHUKS10",
        "WELCOME2026",
        "TRUEMINDS"
    ];

    private readonly AppDbContext _db;

    public UserBL(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<SignupResponse>> SignupAsync(SignupCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email) && string.IsNullOrWhiteSpace(command.PhoneNumber))
        {
            return ApiResult<SignupResponse>.Fail("Provide either email or phone number.", 400);
        }

        if (!string.IsNullOrWhiteSpace(command.ReferralCode) &&
            !ValidReferralCodes.Contains(command.ReferralCode.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            return ApiResult<SignupResponse>.Fail("Invalid or expired referral code.", 400);
        }

        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            var exists = await _db.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == command.Email.Trim().ToLower(), cancellationToken);
            if (exists)
            {
                return ApiResult<SignupResponse>.Fail("Email already exists.", 409);
            }
        }

        if (!string.IsNullOrWhiteSpace(command.PhoneNumber))
        {
            var exists = await _db.Users.AnyAsync(u => u.PhoneNumber == command.PhoneNumber.Trim(), cancellationToken);
            if (exists)
            {
                return ApiResult<SignupResponse>.Fail("Phone number already exists.", 409);
            }
        }

        var userId = Guid.NewGuid();
        var otp = Random.Shared.Next(100000, 1000000).ToString();

        _db.Users.Add(new User
        {
            Id = userId,
            Email = Normalize(command.Email),
            PhoneNumber = Normalize(command.PhoneNumber),
            ReferralCode = Normalize(command.ReferralCode),
            IsVerified = false,
            CreatedAtUtc = DateTime.UtcNow
        });

        _db.OtpVerifications.Add(new OtpVerification
        {
            UserId = userId,
            Code = otp,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5)
        });

        await _db.SaveChangesAsync(cancellationToken);

        return ApiResult<SignupResponse>.Ok(new SignupResponse("Signup created. Verify account with OTP.", userId, otp));
    }

    public async Task<ApiResult<VerifyResponse>> VerifyAsync(VerifyUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user is null)
        {
            return ApiResult<VerifyResponse>.Fail("User not found.", 404);
        }

        var otpRecord = await _db.OtpVerifications.FirstOrDefaultAsync(o => o.UserId == command.UserId, cancellationToken);
        if (otpRecord is null)
        {
            return ApiResult<VerifyResponse>.Fail("No active verification request.", 400);
        }

        if (DateTime.UtcNow > otpRecord.ExpiresAtUtc)
        {
            _db.OtpVerifications.Remove(otpRecord);
            await _db.SaveChangesAsync(cancellationToken);
            return ApiResult<VerifyResponse>.Fail("OTP expired.", 400);
        }

        if (!string.Equals(otpRecord.Code, command.Otp?.Trim(), StringComparison.Ordinal))
        {
            return ApiResult<VerifyResponse>.Fail("Invalid OTP.", 400);
        }

        user.IsVerified = true;
        _db.OtpVerifications.Remove(otpRecord);
        await _db.SaveChangesAsync(cancellationToken);

        return ApiResult<VerifyResponse>.Ok(new VerifyResponse("User verified successfully.", command.UserId));
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
