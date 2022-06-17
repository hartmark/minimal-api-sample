using Asp.Versioning;
using Asp.Versioning.Builder;
using Entities.DataContract.ErrorResponse;

namespace Api;

public static class RouteHandlerBuilderExtensions
{
    public static void AddDefaultMappingBehaviour(this RouteHandlerBuilder builder, ApiVersionSet versionSet, string tag)
    {
        builder
            .Produces<BadRequestResponse>(StatusCodes.Status400BadRequest)
            .Produces<ClientErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ClientErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ClientErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<InternalServerErrorResponse>(StatusCodes.Status500InternalServerError)
            .WithTags(tag)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(new ApiVersion(1.0));
    }
}