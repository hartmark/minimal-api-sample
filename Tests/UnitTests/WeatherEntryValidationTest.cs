using System;
using System.Linq;
using Entities.DataContract;
using Entities.Enums;
using FluentValidation;
using Infrastructure.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.UnitTests;

[Trait("Category", "CI")]
public class WeatherEntryValidationTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IValidator<WeatherEntry> _validator;

    public WeatherEntryValidationTest(ITestOutputHelper testOutputHelper, IValidator<WeatherEntry> validator)
    {
        _testOutputHelper = testOutputHelper;
        _validator = validator;
    }

    [Fact]
    public void Validate_WeatherEntry_ShouldBeInvalid_WhenHavingObservedTimeInFuture()
    {
        var weatherEntry = new WeatherEntry(0, DateTime.Now.AddDays(1), TemperatureType.Celsius);

        var validationResult = _validator.Validate(weatherEntry);
        _testOutputHelper.WriteLine(validationResult.Errors.ToJson(true));
        
        Assert.False(validationResult.IsValid, "Unexpected valid");

        Assert.Single(validationResult.Errors.Where(x => x.PropertyName == nameof(weatherEntry.ObservedTime)));
    }
    
    [Fact]
    public void Validate_WeatherEntry_ShouldBeValid_WhenObjectIsValid()
    {
        var weatherEntry = new WeatherEntry(0, DateTime.Now.AddDays(-1), TemperatureType.Celsius);

        var validationResult = _validator.Validate(weatherEntry);
        _testOutputHelper.WriteLine(validationResult.Errors.ToJson(true));
        
        Assert.True(validationResult.IsValid, "Unexpected valid");

        Assert.Empty(validationResult.Errors);
    }
}