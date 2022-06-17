namespace Entities.DataContract.ErrorResponse;

public class ErrorDetail
{
    public ErrorDetail(Exception exception)
    {
        Message = exception.Message;
        StackTrace = exception.StackTrace;
        FullDetails = exception.ToString();
    }

    public string Message { get; }
    public string StackTrace { get; }
    public string FullDetails { get; }
}