using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests.ApiTests;

public class TestDataGenerator : IEnumerable<object[]>
{

    public IEnumerator<object[]> GetEnumerator() 
    {
        var api = new WebApplicationFactory<ApiProgram>();
        var weatherRepositoryMock = Substitute.For<IWeatherRepository>();
        var client = GetHttpClientForProgram(api, weatherRepositoryMock);
        yield return new object[] { new TestHost("Api", client, weatherRepositoryMock) };

        var apiClassic = new WebApplicationFactory<ApiClassicProgram>();
        weatherRepositoryMock = Substitute.For<IWeatherRepository>();
        var clientClassic = GetHttpClientForProgram(apiClassic, weatherRepositoryMock);
        yield return new object[] { new TestHost("ApiClassic", clientClassic, weatherRepositoryMock) };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    
    private HttpClient GetHttpClientForProgram<TProgram>(
        WebApplicationFactory<TProgram> program,
        IWeatherRepository weatherRepositoryMock) where TProgram : class
    {
        var host = program
            .WithWebHostBuilder(builder =>
            {
                builder
                    .UseContentRoot(Path.GetDirectoryName(GetType().Assembly.Location)!)
                    .ConfigureAppConfiguration(
                        configurationBuilder =>
                            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(_ => weatherRepositoryMock);
                        services.AddLogging(logging => logging.AddXUnit());
                    });
            });

        return host.CreateClient();
    }
}