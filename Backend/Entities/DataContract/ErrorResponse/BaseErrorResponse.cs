using System.Net;
using System.Text.Json.Serialization;

namespace Entities.DataContract.ErrorResponse;

public abstract class BaseErrorResponse
{
    protected BaseErrorResponse(string type, string title, HttpStatusCode status)
    {
        Type = type;
        Title = title;
        Status = status;
    }

    protected BaseErrorResponse()
    {
    }

    [JsonInclude]
    public string Type { get; set; }
    
    [JsonInclude]
    public string Title { get; set; }
    
    [JsonInclude]
    public HttpStatusCode Status { get; set; }
    
}
