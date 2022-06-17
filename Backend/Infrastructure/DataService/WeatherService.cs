using System.Net;
using Entities.DataContract;
using Entities.Exceptions;
using Infrastructure.DataAccess;

namespace Infrastructure.DataService;

public class WeatherService : IWeatherService
{
    private readonly IWeatherRepository _weatherRepository;

    public WeatherService(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }
    
    public void DeleteWeatherEntry(int id)
    {
        
        if (_weatherRepository.Get(id) == null)
        {
            throw new ApiCallException($"Weather entry not found, id: {id}", HttpStatusCode.NotFound);
        }

        _weatherRepository.Delete(id);
    }

    public int CreateWeatherEntry(WeatherEntry weatherEntry)
    {
        return _weatherRepository.Create(weatherEntry);
    }
    
    public WeatherEntry GetWeatherEntry(int id)
    {
        var weatherEntry = _weatherRepository.Get(id);
        if (weatherEntry == null)
        {
            throw new ApiCallException($"Weather entry not found, id: {id}", HttpStatusCode.NotFound);
        }

        return weatherEntry;
    }

    public IEnumerable<WeatherEntry> GetWeatherEntries()
    {
        return _weatherRepository.GetAll();
    }

    public void UpdateWeatherEntry(int id, WeatherEntry weatherEntry)
    {
        if (_weatherRepository.Get(id) == null)
        {
            throw new ApiCallException($"Weather entry not found, id: {id}", HttpStatusCode.NotFound);
        }

        _weatherRepository.Update(id, weatherEntry);
    }
}