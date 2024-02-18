namespace RaffleKing.Common;

public class OperationResult
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;
    
    // Factory method for a successful operation
    public static OperationResult Ok(string message = "") => new OperationResult { Success = true, Message = message };
    
    // Factory method for a failed operation
    public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
    
    // Private constructor to enforce use of factory methods
    private OperationResult() {}
}