using Entities.DataContract;
using FluentValidation;

namespace ApiClassic.Validation;

// ReSharper disable once ClassNeverInstantiated.Global - Used internally by FluentValidation
public class WeatherEntryValidator : AbstractValidator<WeatherEntry>
{
    public WeatherEntryValidator()
    {
        RuleFor(weatherEntry => weatherEntry.ObservedTime)
            .InclusiveBetween(new DateTime(1900, 1, 1), DateTime.Now);

        RuleFor(weatherEntry => weatherEntry.TemperatureType)
            .IsInEnum();
    }
}