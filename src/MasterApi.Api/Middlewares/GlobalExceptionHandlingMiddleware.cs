using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MasterApi.Api.Middlewares;

public class GlobalExceptionHandlingMiddleware : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "An unhandled exception has occurred. TraceId: {TraceId}, Message: {Message}",
            traceId,
            exception.Message);

        var (statusCode, title, detail) = GetProblemDetails(exception);
        
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        problemDetails.Extensions.Add("traceId", traceId);

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }

    private (int StatusCode, string Title, string Detail) GetProblemDetails(Exception exception)
    {
        // Here you could add more specific exception handling if needed
        return
        (
            StatusCodes.Status500InternalServerError,
            "Server Error",
            "An unexpected internal server error has occurred."
        );
    }
}
