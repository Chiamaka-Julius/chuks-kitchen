using Chuks_Kitchen.API.Contracts;
using Chuks_Kitchen.API.Extensions;
using Chuks_Kitchen.Services.BL.Interfaces;
using Chuks_Kitchen.Services.CQRS;
using Chuks_Kitchen.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Chuks_Kitchen.API.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserBL _userBl;
    private readonly ICqrsDispatcher _cqrs;

    public UsersController(IUserBL userBl, ICqrsDispatcher cqrs)
    {
        _userBl = userBl;
        _cqrs = cqrs;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(SignupRequest request, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteCommandAsync(
            ct => _userBl.SignupAsync(new SignupCommand(request.Email, request.PhoneNumber, request.ReferralCode), ct),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(VerifyRequest request, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteCommandAsync(
            ct => _userBl.VerifyAsync(new VerifyUserCommand(request.UserId, request.Otp), ct),
            cancellationToken);

        return result.ToActionResult();
    }
}
