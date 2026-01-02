using MasterApi.Domain.Entities;

namespace MasterApi.Application.Auditing;

public class AuditEntry
{
    public DateTime TimestampUtc { get; set; }
    public Guid? UserId { get; set; }
    public UserRole? UserRole { get; set; }
    public List<string> Permissions { get; set; } = new List<string>();
    public string HttpMethod { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
}
