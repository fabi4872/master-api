using MasterApi.Application.Abstractions.Auditing;
using MasterApi.Application.Auditing;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using System.Text.Json;

namespace MasterApi.Infrastructure.Auditing;

public class BusinessAuditService : IBusinessAuditService
{
    private readonly ILogger<BusinessAuditService> _logger;

    public BusinessAuditService(ILogger<BusinessAuditService> logger)
    {
        _logger = logger;
    }

    public Task WriteAsync(BusinessAuditEvent entry)
    {
        using (LogContext.PushProperty("EventType", entry.EventType))
        using (LogContext.PushProperty("UserId", entry.UserId))
        using (LogContext.PushProperty("CorrelationId", entry.CorrelationId))
        using (LogContext.PushProperty("Metadata", JsonSerializer.Serialize(entry.Metadata)))
        {
            _logger.LogInformation(
                "Business Audit Event: Type={EventType}, UserId={UserId}, CorrelationId={CorrelationId}, Metadata={Metadata}",
                entry.EventType,
                entry.UserId,
                entry.CorrelationId,
                entry.Metadata
            );
        }
        // TODO: Persist to database
        return Task.CompletedTask;
    }
}
