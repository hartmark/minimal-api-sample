using Entities.DataContract;

namespace Infrastructure.DataAccess;

public class WeatherRepository : IWeatherRepository
{
    private readonly Dictionary<int, WeatherEntry> _weatherEntries = new();

    public WeatherEntry Get(int id)
    {
        return _weatherEntries.ContainsKey(id) ? _weatherEntries[id] : null;
    }

    public void Delete(int id)
    {
        _weatherEntries.Remove(id);
    }

    public int Create(WeatherEntry weatherEntry)
    {
        var id = _weatherEntries.Keys.Any() ? _weatherEntries.Keys.Max() + 1 : 0;
        _weatherEntries.Add(id, weatherEntry);
        return id;
    }

    public IEnumerable<WeatherEntry> GetAll()
    {
        return _weatherEntries.Values;
    }

    public void Update(int id, WeatherEntry weatherEntry)
    {
        _weatherEntries[id] = weatherEntry;
    }
}