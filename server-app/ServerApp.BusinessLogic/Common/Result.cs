namespace ServerApp.BusinessLogic.Common;

/// <summary>
/// Represents the outcome of an operation, including success or failure,
/// and optionally a value or an error message.
/// </summary>
/// <typeparam name="T">The type of the value when operation succeeds.</typeparam>
public class Result<T>
{
    public bool Success { get; set; }

    public bool IsFailed { get => !Success; }

    public List<string> Errors { get; set; } = [];

    public T Value
    {
        get
        {
            if (Success)
                return Value;
            throw new InvalidOperationException(
                "Can't get value if the result is not Success");
        }
    }
}

/// <summary>
/// Represents the outcome of an operation (success or failure)
/// and optionally an error message.
/// </summary>
public class Result
{
    public bool Success { get; }
    public string? Error { get; }

    private Result(bool success, string? error)
    {
        Success = success;
        Error = error;
    }

    public static Result Ok() => new(true, null);
    public static Result Fail(string error) => new(false, error);

    public bool IsFailed => !Success;
}
