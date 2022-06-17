using Entities.DataContract;
using Entities.Enums;
using FluentValidation;
using Infrastructure.DataService;

namespace Api.Apis;

internal class WeatherForecastApi : ApiBase, IWeatherForecastApi
{
    private readonly IWeatherService _weatherService;
    private readonly IValidator<WeatherEntry> _weatherEntryValidator;

    public WeatherForecastApi(ILogger<WeatherForecastApi> logger, IWeatherService weatherService,
        IValidator<WeatherEntry> weatherEntryValidator, IUserService userService) : base(logger, userService)
    {
        _weatherService = weatherService;
        _weatherEntryValidator = weatherEntryValidator;
    }

    public IResult DeleteWeatherEntry(int weatherEntryId, HttpRequest httpRequest)
    {
        return HandleRequest(
            () =>
            {
                _weatherService.DeleteWeatherEntry(weatherEntryId);
                return Results.NoContent();
            }, httpRequest, Role.Write);
    }

    public IResult CreateWeatherEntry(WeatherEntry weatherEntry, HttpRequest httpRequest)
    {
        return HandleRequest(
            () =>
            {
                var validationResult = _weatherEntryValidator.Validate(weatherEntry);
                if (!validationResult.IsValid) return validationResult.ToValidationProblem();

                var id = _weatherService.CreateWeatherEntry(weatherEntry);
                return Results.Ok(new WeatherEntryResponse(id, weatherEntry));
            }, httpRequest, Role.Write);
    }

    public IResult GetWeatherEntry(int weatherEntryId, HttpRequest httpRequest)
    {
        return HandleRequest(
            () =>
            {
                var response = _weatherService.GetWeatherEntry(weatherEntryId);
                return Results.Ok(response);
            }, httpRequest, Role.Read);
    }

    public IResult GetWeatherEntries(HttpRequest httpRequest)
    {
        return HandleRequest(
            () =>
            {
                var weatherEntries = _weatherService.GetWeatherEntries();
                return Results.Ok(weatherEntries);
            }, httpRequest, Role.Read);
    }

    public IResult UpdateWeatherEntry(int id, WeatherEntry weatherEntry, HttpRequest httpRequest)
    {
        return HandleRequest(
            () =>
            {
                var validationResult = _weatherEntryValidator.Validate(weatherEntry);
                if (!validationResult.IsValid) return validationResult.ToValidationProblem();

                _weatherService.UpdateWeatherEntry(id, weatherEntry);
                return Results.NoContent();
            }, httpRequest, Role.Write);
    }
}