using System.Net;

namespace Entities.DataContract.ErrorResponse;

// ReSharper disable once ClassNeverInstantiated.Global - Used for deserializing
public class BadRequestResponse : BaseErrorResponse
{
    public BadRequestResponse(Dictionary<string, List<string>> errors) : base(
        "urn:weather:status:validationError:v1", "Validation error", HttpStatusCode.BadRequest)
    {
        Errors = errors;
    }

    public Dictionary<string, List<string>> Errors { get; }
}