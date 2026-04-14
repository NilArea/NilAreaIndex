namespace NilArea.Contracts.Shared;

public record BloomReserveResult
{
    private BloomReserveResult(bool isSuccess, string? errorCode = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }

    public static BloomReserveResult Success()
    {
        return new BloomReserveResult(true);
    }

    public static BloomReserveResult AlreadyExists(string msg, bool asSuccess)
    {
        return new BloomReserveResult(asSuccess, "EXISTS", msg);
    }

    public static BloomReserveResult InvalidArguments(string msg)
    {
        return new BloomReserveResult(false, "INVALID_ARGS", msg);
    }

    public static BloomReserveResult UnknownError(string msg)
    {
        return new BloomReserveResult(false, "UNKNOWN", msg);
    }
}