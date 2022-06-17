using System.Net;
using System.Text.Json.Serialization;

namespace Entities.DataContract.ErrorResponse;

public class InternalServerErrorResponse : BaseErrorResponse
{
    public InternalServerErrorResponse(Exception exception) : base("urn:weather:status:serviceError:v1",
        "Service error", HttpStatusCode.InternalServerError)
    {
        ErrorDetail = new ErrorDetail(exception);
    }

    [JsonInclude]
    public ErrorDetail ErrorDetail { get; }
}