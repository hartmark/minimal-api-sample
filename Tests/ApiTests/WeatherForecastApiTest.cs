using System;
using System.Net;
using System.Threading.Tasks;
using Entities.DataContract;
using Entities.Enums;
using NSubstitute;
using Tests.Extensions;
using Xunit;
using Xunit.Abstractions;
using Infrastructure.Extensions;

namespace Tests.ApiTests;

[Trait("Category", "CI")]
public class WeatherForecastApiTest : ApiTestBase
{
    private const string UsernameWithWriteRole = "harre";
    private const string PasswordWithWriteRole = "errah";

    private const string UsernameWithReadRole = "noob";
    private const string PasswordWithReadRole = "password";

    public WeatherForecastApiTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    private async Task<string> GetJwt(string username, string password)
    {
        var (content, statusCode, _) = await Client.GetExtendedAsync<string>(
            $"/api/v1/login?username={username}&password={password}", jwt: null);

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(content);

        return content;
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        const int id = 2;

        var jwt = await GetJwt(UsernameWithWriteRole, PasswordWithWriteRole);
        var (content, httpStatusCode) =
            await Client.DeleteExtendedAsync($"/api/WeatherForecasts/v1/{id}", jwt);

        Assert.Equal(HttpStatusCode.NotFound, httpStatusCode);
        Assert.Contains($"Weather entry not found, id: {id}", content);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoResponse_WhenSuccessful()
    {
        const int id = 2;

        var weatherEntry = new WeatherEntry(0, DateTime.Now.AddDays(1), TemperatureType.Celsius);
        WeatherRepositoryMock.Get(id).Returns(weatherEntry);
        
        var jwt = await GetJwt(UsernameWithWriteRole, PasswordWithWriteRole);
        var (content, httpStatusCode) =
            await Client.DeleteExtendedAsync($"/api/WeatherForecasts/v1/{id}", jwt);

        Assert.Equal(HttpStatusCode.NoContent, httpStatusCode);
        Assert.Empty(content);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNewItemIsNotValid()
    {
        var request = new WeatherEntry(0, DateTime.Now.AddDays(1), (TemperatureType)42);

        var jwt = await GetJwt(UsernameWithWriteRole, PasswordWithWriteRole);
        var (content, httpStatusCode, badRequestResponse) =
            await Client.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);

        TestOutputHelper.WriteLine(content);

        Assert.Equal(HttpStatusCode.BadRequest, httpStatusCode);
        Assert.NotNull(badRequestResponse);
        
        Assert.Collection(badRequestResponse.Errors.Keys,
            x => Assert.Equal("ObservedTime", x),
            x => Assert.Equal("TemperatureType", x));
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenNewItemIsValid()
    {
        var request = new WeatherEntry(42, DateTime.Now.AddDays(-1), TemperatureType.Fahrenheit);

        var jwt = await GetJwt(UsernameWithWriteRole, PasswordWithWriteRole);
        var (content, httpStatusCode, badRequestResponse) =
            await Client.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);

        TestOutputHelper.WriteLine(content);
        Assert.Equal(HttpStatusCode.OK, httpStatusCode);
        Assert.Null(badRequestResponse);
        Assert.NotNull(content);

        var weatherEntry = content.ToObject<WeatherEntry>();
        Assert.NotNull(weatherEntry);
        Assert.Equal(request.Temperature, weatherEntry.Temperature);
        Assert.Equal(request.ObservedTime, weatherEntry.ObservedTime);
        Assert.Equal(request.TemperatureType, weatherEntry.TemperatureType);
    }
    
    [Fact]
    public async Task Create_ShouldReturnForbidden_WhenUserLacksWriteRole()
    {
        var jwt = await GetJwt(UsernameWithReadRole, PasswordWithReadRole);
        var request = new WeatherEntry(0, DateTime.Now.AddDays(-1), TemperatureType.Celsius);

        var (_, httpStatusCode, _) = await Client.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);

        Assert.Equal(HttpStatusCode.Forbidden, httpStatusCode);
    }
    
    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenJwtIsBroken()
    {
        const string jwt = "broken";
        var request = new WeatherEntry(0, DateTime.Now.AddDays(-1), TemperatureType.Celsius);

        var (_, httpStatusCode, _) = await Client.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenRequestingNonExistingItem()
    {
        const int id = 1;

        var jwt = await GetJwt(UsernameWithWriteRole, PasswordWithWriteRole);
        var (_, statusCode, _) =
            await Client.GetExtendedAsync<WeatherEntry>($"/api/WeatherForecasts/v1/{id}", jwt);
        Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WhenRequestingExitingItem()
    {
        const int id = 2;
        var weatherEntry = new WeatherEntry(0, DateTime.Now.AddDays(1), TemperatureType.Celsius);
        WeatherRepositoryMock.Get(id).Returns(weatherEntry);

        var jwt = await GetJwt(UsernameWithWriteRole, PasswordWithWriteRole);
        var (_, statusCode, _) =
            await Client.GetExtendedAsync<WeatherEntry>($"/api/WeatherForecasts/v1/{id}", jwt);

        Assert.Equal(HttpStatusCode.OK, statusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenTryingToUpdateNoneExistingItem()
    {
        const int id = 1;

        var jwt = await GetJwt(UsernameWithWriteRole, PasswordWithWriteRole);
        var request = new WeatherEntry(0, DateTime.Now.AddDays(-1), TemperatureType.Celsius);

        var (_, statusCode, _) =
            await Client.PutExtendedAsync($"/api/WeatherForecasts/v1/{id}", request, jwt);
        Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }
    
    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenTryingToUpdateExistingItem()
    {
        const int id = 2;
        var weatherEntry = new WeatherEntry(0, DateTime.Now.AddDays(1), TemperatureType.Celsius);
        WeatherRepositoryMock.Get(id).Returns(weatherEntry);

        var jwt = await GetJwt(UsernameWithWriteRole, PasswordWithWriteRole);
        var request = new WeatherEntry(0, DateTime.Now.AddDays(-1), TemperatureType.Celsius);

        var (_, statusCode, _) =
            await Client.PutExtendedAsync($"/api/WeatherForecasts/v1/{id}", request, jwt);
        Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }
}