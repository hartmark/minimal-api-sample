using System.Net;
using System.Net.Mime;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ApiClassic;

public class ApiResult : IActionResult
{
    private readonly object _value;
    private readonly HttpStatusCode _httpStatusCode;
    public ApiResult(object value, HttpStatusCode httpStatusCode)
    {
        _value = value;
        _httpStatusCode = httpStatusCode;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)_httpStatusCode;
        context.HttpContext.Response.ContentType = MediaTypeNames.Application.Json;
        await context.HttpContext.Response.WriteAsync(_value.ToJson(true));
    }
}