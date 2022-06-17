using System;
using System.IO;
using System.Net.Http;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace Tests.ApiTests;

public class ApiTestBase : IDisposable
{
    protected ApiTestBase(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
        ResetMocks();
        Initialize();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        ResetMocks();
    }
    
    private WebApplicationFactory<Program> _host = null!;
    protected HttpClient Client { get; private set; } = null!;

    protected IWeatherRepository WeatherRepositoryMock { get; private set; }

    protected ITestOutputHelper TestOutputHelper { get; set; }

    
    
    private void Initialize()
    {
        _host = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder
                    .UseContentRoot(Path.GetDirectoryName(GetType().Assembly.Location)!)
                    .ConfigureAppConfiguration(
                        configurationBuilder =>
                            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(_ => WeatherRepositoryMock);
                        services.AddLogging(logging => logging.AddXUnit(TestOutputHelper));
                    });
            });

        Client = _host.CreateClient();
    }

    private void ResetMocks()
    {
        WeatherRepositoryMock = Substitute.For<IWeatherRepository>();
    }
}