using Api.Apis;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes;

public static class AuthRouteConfiguration
{
    public static void MapAuthRoutes(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        const string routePrefix = "/api";

        app.MapGet($"{routePrefix}/v{{version:apiVersion}}/login",
                 ([FromServices] IAuthApi api, string username, string password) =>
                    api.Login(username, password))
            .Produces<string>()
            .AddDefaultMappingBehaviour(versionSet, "Auth");
    }
}