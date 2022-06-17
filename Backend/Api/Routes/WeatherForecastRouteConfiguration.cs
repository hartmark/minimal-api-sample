using Api.Apis;
using Asp.Versioning;
using Entities.DataContract;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes;

public static class WeatherForecastRouteConfiguration
{
    public static void MapWeatherForecastRoutes(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        const string routePrefix = "/api";

        app.MapDelete($"{routePrefix}/WeatherForecasts/v{{version:apiVersion}}/{{id:int}}",
                ([FromServices] IWeatherForecastApi api, int id, HttpRequest httpRequest) =>
                    api.DeleteWeatherEntry(id, httpRequest))
            .Produces<NoContentResult>(StatusCodes.Status204NoContent)
            .AddDefaultMappingBehaviour(versionSet, "WeatherForecast");

        app.MapPut($"{routePrefix}/WeatherForecasts/v{{version:apiVersion}}/{{id:int}}",
                ([FromServices] IWeatherForecastApi api, int id,
                    WeatherEntry weatherEntry, HttpRequest httpRequest
                ) => api.UpdateWeatherEntry(id, weatherEntry, httpRequest))
            .Produces<EmptyResult>(StatusCodes.Status204NoContent)
            .AddDefaultMappingBehaviour(versionSet, "WeatherForecast");

        app.MapPost($"{routePrefix}/WeatherForecasts/v{{version:apiVersion}}",
                ([FromServices] IWeatherForecastApi api, WeatherEntry weatherForecast,
                        HttpRequest httpRequest) =>
                    api.CreateWeatherEntry(weatherForecast, httpRequest))
            .Produces<EmptyResult>(StatusCodes.Status204NoContent)
            .AddDefaultMappingBehaviour(versionSet, "WeatherForecast");

        app.MapGet($"{routePrefix}/WeatherForecasts/v{{version:apiVersion}}/{{id:int}}",
                ([FromServices] IWeatherForecastApi api, int id, HttpRequest httpRequest) =>
                    api.GetWeatherEntry(id, httpRequest))
            .Produces<WeatherEntry>()
            .AddDefaultMappingBehaviour(versionSet, "WeatherForecast");

        app.MapGet($"{routePrefix}/WeatherForecasts/v{{version:apiVersion}}",
                ([FromServices] IWeatherForecastApi api, HttpRequest httpRequest) =>
                    api.GetWeatherEntries(httpRequest))
            .Produces<List<WeatherEntry>>()
            .AddDefaultMappingBehaviour(versionSet, "WeatherForecast");
    }
}