using Chuks_Kitchen.Services.Contracts;

namespace Chuks_Kitchen.Services.CQRS;

public sealed class CqrsDispatcher : ICqrsDispatcher
{
    public async Task<ApiResult<TResult>> ExecuteCommandAsync<TResult>(Func<CancellationToken, Task<ApiResult<TResult>>> action, CancellationToken cancellationToken = default)
    {
        try
        {
            return await action(cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResult<TResult>.Fail($"Command execution failed: {ex.Message}", 500);
        }
    }

    public async Task<ApiResult<TResult>> ExecuteQueryAsync<TResult>(Func<CancellationToken, Task<ApiResult<TResult>>> action, CancellationToken cancellationToken = default)
    {
        try
        {
            return await action(cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResult<TResult>.Fail($"Query execution failed: {ex.Message}", 500);
        }
    }
}
