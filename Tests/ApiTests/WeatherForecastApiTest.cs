using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Entities.DataContract;
using Entities.Enums;
using Humanizer;
using Infrastructure.Extensions;
using NSubstitute;
using Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ApiTests;

[Trait("Category", "CI")]
public class WeatherForecastApiTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const string UsernameWithWriteRole = "harre";
    private const string PasswordWithWriteRole = "errah";

    private const string UsernameWithReadRole = "noob";
    private const string PasswordWithReadRole = "password";

    public WeatherForecastApiTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private static async Task<string> GetJwt(HttpClient client, string username, string password)
    {
        var (content, statusCode, _) = await client.GetExtendedAsync<string>(
            $"/api/v1/login?username={username}&password={password}", jwt: null);

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(content);

        return content;
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Delete_ShouldReturnNotFound_WhenMissing(TestHost testHost)
    {
        const int id = 2;

        var jwt = await GetJwt(testHost.HttpClient, UsernameWithWriteRole, PasswordWithWriteRole);
        var (content, httpStatusCode) =
            await testHost.HttpClient.DeleteExtendedAsync($"/api/WeatherForecasts/v1/{id}", jwt);

        Assert.Equal(HttpStatusCode.NotFound, httpStatusCode);
        Assert.Contains($"Weather entry not found, id: {id}", content);
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Delete_ShouldReturnNoResponse_WhenSuccessful(TestHost testHost)
    {
        const int id = 2;

        var weatherEntry = new WeatherEntry(0, DateTime.Now.AddDays(1), TemperatureType.Celsius);
        testHost.WeatherRepositoryMock.Get(id).Returns(weatherEntry);
        
        var jwt = await GetJwt(testHost.HttpClient, UsernameWithWriteRole, PasswordWithWriteRole);
        var (content, httpStatusCode) =
            await testHost.HttpClient.DeleteExtendedAsync($"/api/WeatherForecasts/v1/{id}", jwt);

        Assert.Equal(HttpStatusCode.NoContent, httpStatusCode);
        Assert.Empty(content);
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Create_ShouldReturnBadRequest_WhenNewItemIsNotValid(TestHost testHost)
    {
        var request = new WeatherEntry(0, DateTime.Now.AddDays(1), (TemperatureType)42);

        var jwt = await GetJwt(testHost.HttpClient, UsernameWithWriteRole, PasswordWithWriteRole);
        var (content, httpStatusCode, badRequestResponse) =
            await testHost.HttpClient.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);

        _testOutputHelper.WriteLine(content);

        Assert.Equal(HttpStatusCode.BadRequest, httpStatusCode);
        Assert.NotNull(badRequestResponse);
        
        Assert.Collection(badRequestResponse.Errors.Keys,
            x => Assert.Equal("ObservedTime", x),
            x => Assert.Equal("TemperatureType", x));
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Create_ShouldReturnOk_WhenNewItemIsValid(TestHost testHost)
    {
        var request = new WeatherEntry(42, DateTime.Now.AddDays(-1), TemperatureType.Fahrenheit);

        var jwt = await GetJwt(testHost.HttpClient, UsernameWithWriteRole, PasswordWithWriteRole);
        var (content, httpStatusCode, badRequestResponse) =
            await testHost.HttpClient.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);

        Assert.Equal(HttpStatusCode.OK, httpStatusCode);
        Assert.Null(badRequestResponse);
        Assert.NotNull(content);

        var weatherEntry = content.ToObject<WeatherEntry>();
        Assert.NotNull(weatherEntry);
        Assert.Equal(request.Temperature, weatherEntry.Temperature);
        Assert.Equal(request.ObservedTime, weatherEntry.ObservedTime);
        Assert.Equal(request.TemperatureType, weatherEntry.TemperatureType);
    }
    
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Create_ShouldReturnForbidden_WhenUserLacksWriteRole(TestHost testHost)
    {
        var jwt = await GetJwt(testHost.HttpClient, UsernameWithReadRole, PasswordWithReadRole);
        var request = new WeatherEntry(0, DateTime.Now.AddDays(-1), TemperatureType.Celsius);

        var (content, httpStatusCode, _) = await testHost.HttpClient.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);

        _testOutputHelper.WriteLine(content);
        Assert.Equal(HttpStatusCode.Forbidden, httpStatusCode);
    }
    
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Create_ShouldReturnUnauthorized_WhenJwtIsBroken(TestHost testHost)
    {
        const string jwt = "broken";
        var request = new WeatherEntry(0, DateTime.Now.AddDays(-1), TemperatureType.Celsius);

        var (_, httpStatusCode, _) = await testHost.HttpClient.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Get_ShouldReturnNotFound_WhenRequestingNonExistingItem(TestHost testHost)
    {
        const int id = 1;

        var jwt = await GetJwt(testHost.HttpClient, UsernameWithWriteRole, PasswordWithWriteRole);
        var (_, statusCode, _) =
            await testHost.HttpClient.GetExtendedAsync<WeatherEntry>($"/api/WeatherForecasts/v1/{id}", jwt);
        Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Get_ShouldReturnOk_WhenRequestingExitingItem(TestHost testHost)
    {
        const int id = 2;
        var weatherEntry = new WeatherEntry(0, DateTime.Now.AddDays(1), TemperatureType.Celsius);
        testHost.WeatherRepositoryMock.Get(id).Returns(weatherEntry);

        var jwt = await GetJwt(testHost.HttpClient, UsernameWithWriteRole, PasswordWithWriteRole);
        var (_, statusCode, _) =
            await testHost.HttpClient.GetExtendedAsync<WeatherEntry>($"/api/WeatherForecasts/v1/{id}", jwt);

        Assert.Equal(HttpStatusCode.OK, statusCode);
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Update_ShouldReturnNotFound_WhenTryingToUpdateNoneExistingItem(TestHost testHost)
    {
        const int id = 1;

        var jwt = await GetJwt(testHost.HttpClient, UsernameWithWriteRole, PasswordWithWriteRole);
        var request = new WeatherEntry(0, DateTime.Now.AddDays(-1), TemperatureType.Celsius);

        var (_, statusCode, _) =
            await testHost.HttpClient.PutExtendedAsync($"/api/WeatherForecasts/v1/{id}", request, jwt);
        Assert.Equal(HttpStatusCode.NotFound, statusCode);
    }
    
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Update_ShouldReturnNoContent_WhenTryingToUpdateExistingItem(TestHost testHost)
    {
        const int id = 2;
        var weatherEntry = new WeatherEntry(0, DateTime.Now.AddDays(1), TemperatureType.Celsius);
        testHost.WeatherRepositoryMock.Get(id).Returns(weatherEntry);

        var jwt = await GetJwt(testHost.HttpClient, UsernameWithWriteRole, PasswordWithWriteRole);
        var request = new WeatherEntry(0, DateTime.Now.AddDays(-1), TemperatureType.Celsius);

        var (_, statusCode, _) =
            await testHost.HttpClient.PutExtendedAsync($"/api/WeatherForecasts/v1/{id}", request, jwt);
        Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }
    
    
    private static async Task CreateRandomEntry(TestHost testHost, string jwt)
    {
        var request = new WeatherEntry(Random.Shared.Next(-50, 50), DateTime.Now.AddDays(Random.Shared.Next(-10, -1)),
            (TemperatureType)Random.Shared.Next(0, 2));

        var (content, httpStatusCode, badRequestResponse) =
            await testHost.HttpClient.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);

        Assert.Equal(HttpStatusCode.OK, httpStatusCode);
        Assert.Null(badRequestResponse);
        Assert.NotNull(content);

        var weatherEntry = content.ToObject<WeatherEntry>();
        Assert.NotNull(weatherEntry);
        Assert.Equal(request.Temperature, weatherEntry.Temperature);
        Assert.Equal(request.ObservedTime, weatherEntry.ObservedTime);
        Assert.Equal(request.TemperatureType, weatherEntry.TemperatureType);
    }
    
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public async Task Spam_Create_Test(TestHost testHost)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var jwt = await GetJwt(testHost.HttpClient, UsernameWithWriteRole, PasswordWithWriteRole);

        const int calls = 1000;
        const int threads = 15;
        foreach (var _ in Enumerable.Range(1, calls))
        {
            var tasks = new Task[threads];
            for (var threadIndex = 0; threadIndex < threads; threadIndex++)
            {
                tasks[threadIndex] = CreateRandomEntry(testHost, jwt);
            }

            Task.WaitAll(tasks);
        }

        var elapsed = stopwatch.Elapsed;
        _testOutputHelper.WriteLine("Executed {0} calls in {1}", calls*threads, elapsed.Humanize());
        _testOutputHelper.WriteLine("{0} seconds", elapsed.TotalSeconds);
        _testOutputHelper.WriteLine("{0} calls/s", calls*threads/elapsed.TotalSeconds);
    }
}