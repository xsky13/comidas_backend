namespace comidas_backend.Utils;

public class Result<T>
{
    public bool Success { get; set; }
    public T? Value { get; set; }
    public string? Error { get; set; }
    public int StatusCode { get; set; }
    
    private Result() { }

    public static Result<T> Ok(T value) => new Result<T>() { Success = true, Value = value, StatusCode = 200};
    public static Result<T> Fail(string error, int? code) => new Result<T>() { Success = false, Error = error, StatusCode = code ?? 400 };
    
}