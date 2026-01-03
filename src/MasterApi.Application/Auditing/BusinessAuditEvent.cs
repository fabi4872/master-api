using System.Collections.Generic;
using MasterApi.Domain.Entities;

namespace MasterApi.Application.Auditing;

public class BusinessAuditEvent
{
    public string EventType { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; }
    public Guid? UserId { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    public string CorrelationId { get; set; } = string.Empty;
}
