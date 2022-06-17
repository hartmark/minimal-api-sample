using Api.Middleware;

namespace Api;

public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder UseJwtAuthentication(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtAuthentication>();
    }
}