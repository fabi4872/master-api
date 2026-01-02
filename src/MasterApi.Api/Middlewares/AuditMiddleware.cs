using System.Security.Claims;
using MasterApi.Application.Abstractions.Auditing;
using MasterApi.Application.Auditing;
using MasterApi.Domain.Entities;
using Serilog.Context;

namespace MasterApi.Api.Middlewares;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditService auditService)
    {
        var auditEntry = new AuditEntry
        {
            TimestampUtc = DateTime.UtcNow,
            HttpMethod = context.Request.Method,
            Path = context.Request.Path,
            CorrelationId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier
        };

        var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            auditEntry.UserId = userId;
        }

        var userRoleClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (Enum.TryParse<UserRole>(userRoleClaim, out var userRole))
        {
            auditEntry.UserRole = userRole;
        }

        auditEntry.Permissions.AddRange(context.User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value));
        
        using (LogContext.PushProperty("CorrelationId", auditEntry.CorrelationId))
        using (LogContext.PushProperty("UserId", auditEntry.UserId))
        using (LogContext.PushProperty("UserRole", auditEntry.UserRole))
        using (LogContext.PushProperty("Permissions", auditEntry.Permissions))
        {
            await _next(context);

            auditEntry.StatusCode = context.Response.StatusCode;
            await auditService.WriteAsync(auditEntry);
        }
    }
}
