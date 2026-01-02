using MasterApi.Application.Abstractions.Auditing;
using MasterApi.Application.Auditing;
using Microsoft.Extensions.Logging;

namespace MasterApi.Infrastructure.Auditing;

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
    }

    public Task WriteAsync(AuditEntry entry)
    {
        // Log the audit entry using Serilog
        _logger.LogInformation(
            "Audit Entry: Timestamp={Timestamp}, UserId={UserId}, UserRole={UserRole}, Permissions={Permissions}, Method={HttpMethod}, Path={Path}, StatusCode={StatusCode}, CorrelationId={CorrelationId}",
            entry.TimestampUtc,
            entry.UserId,
            entry.UserRole,
            string.Join(",", entry.Permissions),
            entry.HttpMethod,
            entry.Path,
            entry.StatusCode,
            entry.CorrelationId
        );

        return Task.CompletedTask;
    }
}
