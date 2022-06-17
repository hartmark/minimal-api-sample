using Entities.DataContract;

namespace Api.Apis;

public interface IWeatherForecastApi
{
    IResult DeleteWeatherEntry(int weatherEntryId, HttpRequest httpRequest);
    IResult CreateWeatherEntry(WeatherEntry weatherEntry, HttpRequest httpRequest);
    IResult GetWeatherEntry(int weatherEntryId, HttpRequest httpRequest);
    IResult GetWeatherEntries(HttpRequest httpRequest);
    IResult UpdateWeatherEntry(int entry, WeatherEntry weatherEntry, HttpRequest httpRequest);
}