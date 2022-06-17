using Entities.DataContract;

namespace Infrastructure.DataService;

public interface IWeatherService
{
    void DeleteWeatherEntry(int id);
    int CreateWeatherEntry(WeatherEntry weatherEntry);
    WeatherEntry GetWeatherEntry(int id);
    IEnumerable<WeatherEntry> GetWeatherEntries();
    void UpdateWeatherEntry(int id, WeatherEntry weatherEntry);
}