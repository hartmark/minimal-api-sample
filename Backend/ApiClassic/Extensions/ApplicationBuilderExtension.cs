using ApiClassic.Middleware;

namespace ApiClassic.Extensions;

public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder UseJwtAuthentication(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtAuthentication>();
    }
}