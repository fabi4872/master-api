using MasterApi.Application.Abstractions;
using MasterApi.Domain.Core;
using MasterApi.Domain.Errors;
using MasterApi.Domain.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace MasterApi.Api.Controllers;

[ApiController]
public class ApiControllerBase : ControllerBase
{
    protected ILocalizationService Localization =>
    HttpContext.RequestServices.GetRequiredService<ILocalizationService>();

    protected IActionResult HandleFailure(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        if (result.Error == DomainErrors.Request.NotFound)
        {
            return NotFound(CreateProblemDetails(
                "Not Found", 
                StatusCodes.Status404NotFound,
                result.Error));
        }
        
        if (result.Error == DomainErrors.User.InvalidCredentials)
        {
            return Unauthorized(CreateProblemDetails(
                "Unauthorized",
                StatusCodes.Status401Unauthorized,
                result.Error));
        }
        if (result.Error == DomainErrors.User.EmailAlreadyExists)
        {
            return Conflict(CreateProblemDetails(
                "Conflict",
                StatusCodes.Status409Conflict,
                result.Error));
        }
        
        return BadRequest(CreateProblemDetails(
            "Bad Request",
            StatusCodes.Status400BadRequest,
            result.Error));
    }

    private ProblemDetails CreateProblemDetails(
        string title,
        int status,
        Error error,
        Exception? exception = null)
    {
        var message = Localization.GetString(error.Code);

        if (string.IsNullOrEmpty(message))
        {
            message = error.Description;
        }

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = status,
            Detail = message,
            Type = $"https://httpstatuses.com/{status}"
        };
        
        if (exception is not null)
        {
            problemDetails.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        return problemDetails;
    }
}
