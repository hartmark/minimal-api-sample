using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace ApiClassic.Extensions;

public static class ValidationResultExtension
{
    public static IActionResult ToValidationProblem(this ValidationResult validationResult)
    {
        return new BadRequestObjectResult(validationResult.Errors
            .ToLookup(key => key.PropertyName, value => value.ErrorMessage)
            .ToDictionary(key => key.Key, value => value.ToArray()));
    }
}