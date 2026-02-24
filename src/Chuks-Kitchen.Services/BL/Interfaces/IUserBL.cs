using Chuks_Kitchen.Services.Contracts;
using Chuks_Kitchen.Services.Users;

namespace Chuks_Kitchen.Services.BL.Interfaces;

public interface IUserBL
{
    Task<ApiResult<SignupResponse>> SignupAsync(SignupCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult<VerifyResponse>> VerifyAsync(VerifyUserCommand command, CancellationToken cancellationToken = default);
}
