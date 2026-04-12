using System.Text.Json;
using Carbon.Audit.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Carbon.Audit.Producer;

public sealed class RabbitMqAuditEventPublisher : IAuditEventPublisher
{
    private readonly IPublishEndpoint _bus;
    private readonly ILogger<RabbitMqAuditEventPublisher> _logger;

    public RabbitMqAuditEventPublisher(
        IPublishEndpoint bus,
        ILogger<RabbitMqAuditEventPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public Task PublishAsync(AuditEvent evt)
        => PublishBatchAsync(new[] { evt });

    public Task PublishBatchAsync(IEnumerable<AuditEvent> events)
    {
        var list = events?.ToList() ?? new List<AuditEvent>();

        _ = Task.Run(async () =>
        {
            try
            {
                foreach (var evt in list)
                    await _bus.Publish(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AUDIT_FALLBACK] {Events}", JsonSerializer.Serialize(list));
            }
        });

        return Task.CompletedTask;
    }
}