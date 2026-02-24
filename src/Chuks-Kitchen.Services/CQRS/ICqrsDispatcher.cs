using Chuks_Kitchen.Services.Contracts;

namespace Chuks_Kitchen.Services.CQRS;

public interface ICqrsDispatcher
{
    Task<ApiResult<TResult>> ExecuteCommandAsync<TResult>(Func<CancellationToken, Task<ApiResult<TResult>>> action, CancellationToken cancellationToken = default);
    Task<ApiResult<TResult>> ExecuteQueryAsync<TResult>(Func<CancellationToken, Task<ApiResult<TResult>>> action, CancellationToken cancellationToken = default);
}
