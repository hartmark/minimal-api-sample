using Entities.DataContract;

namespace Infrastructure.DataAccess;

public interface IWeatherRepository
{
    WeatherEntry Get(int id);
    void Delete(int id);
    int Create(WeatherEntry weatherEntry);
    IEnumerable<WeatherEntry> GetAll();
    void Update(int id, WeatherEntry weatherEntry);
}