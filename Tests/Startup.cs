using Api.Validation;
using FluentValidation.AspNetCore;
using Infrastructure.DataService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace Tests;

public class Startup
{
    private static IConfiguration Configuration
    {
        get
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
        });

        SetupDependencyInjection(services);
        services.AddFluentValidation(v => v.RegisterValidatorsFromAssemblyContaining<WeatherEntryValidator>());

        services.AddSingleton(_ => services.BuildServiceProvider());
    }

    private static void SetupDependencyInjection(IServiceCollection services)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };
        services.AddTransient(_ => Configuration);
        services.AddTransient<IJwtGenerator, JwtGenerator>();
    }
}