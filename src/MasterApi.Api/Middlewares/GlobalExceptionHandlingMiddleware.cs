using System.Net;
using System.Text.Json;
using MasterApi.Api.Services;
using MasterApi.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace MasterApi.Api.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly LocalizationService _localizationService;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger, LocalizationService localizationService)
    {
        _next = next;
        _logger = logger;
        _localizationService = localizationService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = GetCorrelationId(context);
            _logger.LogError(ex, "An unhandled exception has occurred. CorrelationId: {CorrelationId}", correlationId);

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = _localizationService.GetString(DomainErrors.General.UnspecifiedError),
                Detail = _localizationService.GetString(DomainErrors.General.UnspecifiedError),
                Instance = context.Request.Path
            };
            
            problemDetails.Extensions.Add("correlationId", correlationId);
            
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
    
    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId);
        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
