using System.Security.Cryptography;
using Api.Apis;
using Infrastructure.DataAccess;
using Infrastructure.DataService;
using JWT;
using JWT.Algorithms;
using JWT.Extensions.AspNetCore.Factories;

namespace Api;

public static class ServiceCollectionExtensions
{
    public static void AddDependencyInjection(this IServiceCollection services)
    {
        services.AddTransient<IWeatherForecastApi, WeatherForecastApi>();
        services.AddTransient<IWeatherService, WeatherService>();
        services.AddTransient<IAuthApi, AuthApi>();
        services.AddTransient<IJwtGenerator, JwtGenerator>();
        services.AddTransient<IUserService, UserService>();
        services.AddSingleton(_ => ECDsa.Create());

        services.AddJwtDependencies();
        
        services.AddSingleton<IWeatherRepository, WeatherRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
    }

    private static void AddJwtDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IIdentityFactory, DefaultIdentityFactory>();
        services.AddSingleton<ITicketFactory, DefaultTicketFactory>();
        services.AddSingleton<IJwtDecoder, JwtDecoder>();
        services.AddSingleton<IAlgorithmFactory, ECDSAAlgorithmFactory>();
    }
}