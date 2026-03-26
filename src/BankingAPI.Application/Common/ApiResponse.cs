namespace BankingAPI.Application.Common;

public class ApiResponse<T>
{
    public string RequestId { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ApiResponse<T> Success(T data, string message = "Request successful.") => new()
    {
        RequestId = GenerateRequestId(),
        Code = "00",
        Message = message,
        Data = data
    };

    public static ApiResponse<object> Success(string message = "Request successful.") => new()
    {
        RequestId = GenerateRequestId(),
        Code = "00",
        Message = message,
        Data = null
    };

    public static ApiResponse<T> Failure(string code, string message) => new()
    {
        RequestId = GenerateRequestId(),
        Code = code,
        Message = message,
        Data = default
    };

    private static string GenerateRequestId()
        => Random.Shared.NextInt64(1_000_000_000L, 9_999_999_999L).ToString();
}
