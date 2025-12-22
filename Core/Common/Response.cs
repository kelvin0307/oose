using Core.Common;

public class Response<T>
{
    public bool Success { get; set; }
    public T Result { get; set; }
    public string Message { get; set; }
    public Dictionary<string, string[]> ValidationErrors { get; set; }
    public ResponseStatus Status { get; set; }

    public static Response<T> Ok(T result) => new()
    {
        Success = true,
        Result = result,
        Status = ResponseStatus.Success
    };

    public static Response<T> NotFound(string message) => new()
    {
        Success = false,
        Message = message,
        Status = ResponseStatus.NotFound
    };

    public static Response<T> Fail(string message, ResponseStatus status = ResponseStatus.UnexpectedError) => new()
    {
        Success = false,
        Message = message,
        Status = status
    };

    public static Response<T> ValidationFail(Dictionary<string, string[]> errors) => new()
    {
        Success = false,
        ValidationErrors = errors,
        Status = ResponseStatus.ValidationError
    };
}