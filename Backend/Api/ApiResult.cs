using System.Net;
using System.Net.Mime;
using Infrastructure.Extensions;

namespace Api;

public class ApiResult : IResult
{
    private readonly object _value;
    private readonly HttpStatusCode _httpStatusCode;
    public ApiResult(object value, HttpStatusCode httpStatusCode)
    {
        _value = value;
        _httpStatusCode = httpStatusCode;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = (int)_httpStatusCode;
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        await httpContext.Response.WriteAsync(_value.ToJson(true));
    }
}