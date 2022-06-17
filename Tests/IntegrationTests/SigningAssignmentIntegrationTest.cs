using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Entities.DataContract;
using Entities.Enums;
using Infrastructure.Extensions;
using Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.IntegrationTests;

[Trait("Category", "Integration")]
public class SigningAssignmentIntegrationTest : ApiIntegrationTestBase
{
    public SigningAssignmentIntegrationTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
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
    public async Task AllWeatherForecasts_ShouldWork()
    {
        var jwt = await GetJwt("harre", "errah");

        var request = new WeatherEntry(42, DateTime.Now.AddDays(-1), TemperatureType.Fahrenheit);

        var (_, statusCodeCreate, _) =
            await Client.PostExtendedAsync("/api/WeatherForecasts/v1", request, jwt);
        
        Assert.Equal(HttpStatusCode.OK, statusCodeCreate);

        var (_, statusCode, responseObject) =
            await Client.GetExtendedAsync<List<WeatherEntry>>(
                "/api/WeatherForecasts/v1", jwt);

        Assert.Equal(HttpStatusCode.OK, statusCode);

        TestOutputHelper.WriteLine(responseObject.ToJson(true));
        Assert.Single(responseObject);
    }
}