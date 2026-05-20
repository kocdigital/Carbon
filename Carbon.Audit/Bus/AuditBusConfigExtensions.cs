using Carbon.Audit.Contracts;
using MassTransit.RabbitMqTransport;

namespace Carbon.Audit.Bus;

/// <summary>
/// Extension methods for configuring audit event routing on a RabbitMQ bus factory.
/// </summary>
public static class AuditBusConfigExtensions
{
    /// <summary>
    /// Configures the RabbitMQ exchange names and types for <see cref="AuditEvent"/>
    /// and <see cref="HttpRequestAuditEvent"/> messages.
    /// Call this inside your <c>UsingRabbitMq</c> / <c>AddRabbitMqBus</c> configurator.
    /// </summary>
    public static void ConfigureAuditPublisher(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<AuditEvent>(m => m.SetEntityName("audit.events"));
        cfg.Publish<AuditEvent>(p => p.ExchangeType = "fanout");

        cfg.Message<HttpRequestAuditEvent>(m => m.SetEntityName("audit.httprequests"));
        cfg.Publish<HttpRequestAuditEvent>(p => p.ExchangeType = "fanout");
    }
}
