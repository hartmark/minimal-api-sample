using ApiClassic.Extensions;
using Entities.DataContract;
using Entities.Enums;
using FluentValidation;
using Infrastructure.DataService;
using Microsoft.AspNetCore.Mvc;

namespace ApiClassic.Controllers;

[ApiController]
[Route("/api/[controller]/v1/")]
public class WeatherForecastsController : BaseController
{
    private readonly IWeatherService _weatherService;
    private readonly IValidator<WeatherEntry> _weatherEntryValidator;

    public WeatherForecastsController(ILogger<WeatherForecastsController> logger, IWeatherService weatherService,
        IValidator<WeatherEntry> weatherEntryValidator, IUserService userService) : base(logger, userService)
    {
        _weatherService = weatherService;
        _weatherEntryValidator = weatherEntryValidator;
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteWeatherEntry(int id)
    {
        return HandleRequest(
            () =>
            {
                _weatherService.DeleteWeatherEntry(id);
                return NoContent();
            }, Request, Role.Write);
    }

    [HttpPost]
    public IActionResult CreateWeatherEntry(WeatherEntry weatherEntry)
    {
        return HandleRequest(
            () =>
            {
                var validationResult = _weatherEntryValidator.Validate(weatherEntry);
                if (!validationResult.IsValid) return validationResult.ToValidationProblem();

                var id = _weatherService.CreateWeatherEntry(weatherEntry);
                return Ok(new WeatherEntryResponse(id, weatherEntry));
            }, Request, Role.Write);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetWeatherEntry(int id)
    {
        return HandleRequest(
            () =>
            {
                var response = _weatherService.GetWeatherEntry(id);
                return Ok(response);
            }, Request, Role.Read);
    }

    [HttpGet]
    public IActionResult GetWeatherEntries()
    {
        return HandleRequest(
            () =>
            {
                var weatherEntries = _weatherService.GetWeatherEntries();
                return Ok(weatherEntries);
            }, Request, Role.Read);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateWeatherEntry(int id, WeatherEntry weatherEntry)
    {
        return HandleRequest(
            () =>
            {
                var validationResult = _weatherEntryValidator.Validate(weatherEntry);
                if (!validationResult.IsValid) return validationResult.ToValidationProblem();

                _weatherService.UpdateWeatherEntry(id, weatherEntry);
                return NoContent();
            }, Request, Role.Write);
    }
}