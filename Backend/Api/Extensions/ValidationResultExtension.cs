using FluentValidation.Results;

namespace Api;

public static class ValidationResultExtension
{
    public static IResult ToValidationProblem(this ValidationResult validationResult)
    {
        return Results.ValidationProblem(validationResult.Errors
            .ToLookup(key => key.PropertyName, value => value.ErrorMessage)
            .ToDictionary(key => key.Key, value => value.ToArray()));
    }
}