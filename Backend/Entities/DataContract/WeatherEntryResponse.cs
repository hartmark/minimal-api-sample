using System.Text.Json.Serialization;

namespace Entities.DataContract;

public record WeatherEntryResponse : WeatherEntry
{
    public WeatherEntryResponse(int id, WeatherEntry weatherEntry) : base(weatherEntry.Temperature, weatherEntry.ObservedTime, weatherEntry.TemperatureType)
    {
        Id = id;
        Temperature = weatherEntry.Temperature;
        ObservedTime = weatherEntry.ObservedTime;
        TemperatureType = weatherEntry.TemperatureType;
    }

    [JsonInclude] public int Id { get; set; }
}