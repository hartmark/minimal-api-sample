using System.Security.Cryptography;
using Infrastructure.DataAccess;
using Infrastructure.DataService;
using JWT;
using JWT.Algorithms;
using JWT.Extensions.AspNetCore.Factories;

namespace ApiClassic.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDependencyInjection(this IServiceCollection services)
    {
        services.AddTransient<IWeatherService, WeatherService>();
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