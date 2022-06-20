using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using Entities.DataContract.ErrorResponse;
using Entities.Enums;
using Entities.Exceptions;
using Humanizer;
using Infrastructure.DataService;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ApiClassic.Controllers;

public abstract class BaseController : ControllerBase
{
    private readonly ILogger _logger;
    protected readonly IUserService UserService;

    protected BaseController(ILogger logger, IUserService userService)
    {
        _logger = logger;
        UserService = userService;
    }

    protected IActionResult HandleRequest(Func<IActionResult> func,
        HttpRequest httpRequest, Role requestedRole, [CallerMemberName] string memberName = "")
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var username = GetUsername(httpRequest);
        _logger.LogDebug(
            "User: {User}, started processing of {ClassName}.{MemberName}", username, GetType().Name, memberName);

        object? response = null;
        try
        {
            AssertRoleMembership(username, requestedRole);
            
            return func.Invoke();
        }
        catch (ApiCallException apiCallException)
        {
            _logger.LogError("\n{ApiCallException}", apiCallException.ToJson(true));

            if ((int)apiCallException.HttpStatusCode is >= 401 and <= 499)
            {
                response = new ClientErrorResponse(apiCallException, httpRequest.Path);
            }
            else
            {
                response = new InternalServerErrorResponse(apiCallException);
            }

            return new ApiResult(response, apiCallException.HttpStatusCode);
        }
        catch (Exception exception)
        {
            _logger.LogError("{Exception}", exception.ToString());
            response = new InternalServerErrorResponse(exception);
            return new ApiResult(response, HttpStatusCode.InternalServerError);
        }
        finally
        {
            _logger.LogDebug("{FinalResponse}", response?.ToJson(true));
            _logger.LogDebug("Finished processing of {ClassName}.{MemberName}, {ElapsedTime}", GetType().Name,
                memberName, stopwatch.Elapsed.Humanize());
        }
    }

    private void AssertRoleMembership(string username, Role requestedRole)
    {
        var roles = UserService.GetRolesForUser(username).ToList();
        if (!roles.Contains(requestedRole))
        {
            throw new ApiCallException($"User {username} are missing required role {requestedRole}, having {roles}",
                HttpStatusCode.Forbidden);
        }
    }

    private static string GetUsername(HttpRequest httpRequest)
    {
        return httpRequest.HttpContext.User.Claims.Single(x => x.Type == "username").Value;
    }
}