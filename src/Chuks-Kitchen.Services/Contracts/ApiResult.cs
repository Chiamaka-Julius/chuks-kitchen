namespace Chuks_Kitchen.Services.Contracts;

public sealed class ApiResult<T>
{
    private ApiResult(bool success, T? data, string? error, int statusCode)
    {
        Success = success;
        Data = data;
        Error = error;
        StatusCode = statusCode;
    }

    public bool Success { get; }
    public T? Data { get; }
    public string? Error { get; }
    public int StatusCode { get; }

    public static ApiResult<T> Ok(T data, int statusCode = 200) => new(true, data, null, statusCode);
    public static ApiResult<T> Fail(string error, int statusCode) => new(false, default, error, statusCode);
}
