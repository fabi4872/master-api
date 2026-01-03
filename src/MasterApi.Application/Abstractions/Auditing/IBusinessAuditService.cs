using MasterApi.Application.Auditing;

namespace MasterApi.Application.Abstractions.Auditing;

public interface IBusinessAuditService
{
    Task WriteAsync(BusinessAuditEvent entry);
}
