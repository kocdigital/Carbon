using MassTransit.RabbitMqTransport;

namespace Carbon.MassTransit;

public static class GlobalConventionsExtensions
{
    public static void ConfigureGlobalMessageMappings(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<Audit.Contracts.AuditEvent>(m => m.SetEntityName("audit.events"));
        cfg.Publish<Audit.Contracts.AuditEvent>(p => p.ExchangeType = "fanout");
    }
}