using System.Net;

namespace Entities.Exceptions;

public class ApiCallException : Exception
{
    public ApiCallException(string message, HttpStatusCode httpStatusCode) : base(message)
    {
        HttpStatusCode = httpStatusCode;
    }

    public HttpStatusCode HttpStatusCode { get; }
}