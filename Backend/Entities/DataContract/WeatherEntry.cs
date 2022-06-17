using System.Text.Json.Serialization;
using Entities.Enums;

namespace Entities.DataContract;

public record WeatherEntry(decimal Temperature, DateTime ObservedTime, TemperatureType TemperatureType)
{
    [JsonInclude]
    public decimal Temperature { get; set; } = Temperature;

    [JsonInclude]
    public DateTime ObservedTime { get; set; } = ObservedTime;

    [JsonInclude]
    public TemperatureType TemperatureType { get; set; } = TemperatureType;

    [JsonInclude]
    public decimal TemperatureAsCelsius => GetTemperature(TemperatureType.Celsius, TemperatureType, Temperature);
    [JsonInclude]
    public decimal TemperatureAsFahrenheit => GetTemperature(TemperatureType.Fahrenheit, TemperatureType, Temperature);
    [JsonInclude]
    public decimal TemperatureAsKelvin => GetTemperature(TemperatureType.Kelvin, TemperatureType, Temperature);

    private static decimal GetTemperature(TemperatureType wanted, TemperatureType actual, decimal temperature)
    {
        switch (actual)
        {
            case TemperatureType.Celsius:
                return wanted switch
                {
                    TemperatureType.Celsius => temperature,
                    TemperatureType.Fahrenheit => temperature * 1.8m + 32,
                    _ => temperature - 273.15m
                };
            case TemperatureType.Fahrenheit:
                return wanted switch
                {
                    TemperatureType.Fahrenheit => temperature,
                    TemperatureType.Celsius => (temperature - 32) / 1.8m,
                    _ => temperature - (temperature + 459.67m) * (5m/9m)
                };
            default: // Kelvin
                return wanted switch
                {
                    TemperatureType.Kelvin => temperature,
                    TemperatureType.Celsius => temperature - 273.15m,
                    _ => temperature - temperature * (9m/5m) - 459.67m
                };
        }
    }
}