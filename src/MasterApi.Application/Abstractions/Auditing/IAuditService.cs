using MasterApi.Application.Auditing;

namespace MasterApi.Application.Abstractions.Auditing;

public interface IAuditService
{
    Task WriteAsync(AuditEntry entry);
}
