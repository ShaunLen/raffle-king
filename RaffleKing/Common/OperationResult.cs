namespace RaffleKing.Common;

public class OperationResult
{
    public bool Success { get; protected init; }
    public string Message { get; protected init; } = string.Empty;
    
    // Factory method for a successful operation
    public static OperationResult Ok(string message = "") => new() { Success = true, Message = message };
    
    // Factory method for a failed operation
    public static OperationResult Fail(string message) => new() { Success = false, Message = message };
    
    // Private constructor to enforce use of factory methods
    protected OperationResult() {}
}

// Variant of OperationResult for returning data with result
public class OperationResult<T> : OperationResult
{
    public T? Data { get; private set; }
    public static OperationResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static OperationResult<T> Fail(string message, T data) => new OperationResult<T>
        { Success = false, Message = message, Data = data };
}