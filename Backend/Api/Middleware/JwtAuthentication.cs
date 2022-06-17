using Microsoft.AspNetCore.Authentication;

namespace Api.Middleware;

public class JwtAuthentication
{
    private readonly RequestDelegate _next;

    public JwtAuthentication(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        var authenticationResult = await httpContext.AuthenticateAsync();
        if (authenticationResult.Succeeded)
        {
            await _next(httpContext);
        }
        else
        {
            if (httpContext.Request.Path.Equals("/api/v1/login") ||
                !httpContext.Request.Path.StartsWithSegments("/api"))
            {
                await _next(httpContext);
            }
            else
            {
                await httpContext.ChallengeAsync();
            }
        }
    }
}