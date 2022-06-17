using System.Net;
using System.Text.Json.Serialization;
using Entities.Exceptions;

namespace Entities.DataContract.ErrorResponse;

/// <summary>
///     Covers HTTP-status codes 400-499
/// </summary>
public class ClientErrorResponse : BaseErrorResponse
{
    
    // ReSharper disable once UnusedMember.Global - Used for deserializing
    public ClientErrorResponse()
    {
    }
    
    public ClientErrorResponse(HttpStatusCode status, string detail, string instance) : base(
        "urn:weather:status:clientError:v1",
        "Client error", status)
    {
        Detail = detail;
        Instance = instance;
    }

    public ClientErrorResponse(ApiCallException apiCallException, string instance) : this(apiCallException.HttpStatusCode,
        apiCallException.ToString(), instance)
    {
    }

    [JsonInclude]
    public string Detail { get; set; }

    [JsonInclude]
    public string Instance { get; set; }
}