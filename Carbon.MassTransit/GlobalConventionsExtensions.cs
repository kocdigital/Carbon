using MassTransit.RabbitMqTransport;
using Carbon.Audit.Contracts;

namespace Carbon.MassTransit
{
    public static class GlobalConventionsExtensions
    {
        public static void ConfigureGlobalMessageMappings(this IRabbitMqBusFactoryConfigurator cfg)
        {
            cfg.Message<AuditEvent>(m => m.SetEntityName("audit.events"));
            cfg.Publish<AuditEvent>(p => p.ExchangeType = "fanout");

            cfg.Message<HttpRequestAuditEvent>(m => m.SetEntityName("audit.httprequests"));
            cfg.Publish<HttpRequestAuditEvent>(p => p.ExchangeType = "fanout");
        }
    }
}