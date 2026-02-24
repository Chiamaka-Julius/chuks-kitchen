namespace Chuks_Kitchen.Services.Users;

public sealed record SignupCommand(string? Email, string? PhoneNumber, string? ReferralCode);
public sealed record VerifyUserCommand(Guid UserId, string Otp);

public sealed record SignupResponse(string Message, Guid UserId, string Otp);
public sealed record VerifyResponse(string Message, Guid UserId);
