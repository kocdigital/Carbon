using Carbon.Audit.Contracts;

namespace Carbon.Audit.Producer;

public interface IAuditEventPublisher
{
    Task PublishAsync(AuditEvent evt);
    Task PublishBatchAsync(IEnumerable<AuditEvent> events);
}