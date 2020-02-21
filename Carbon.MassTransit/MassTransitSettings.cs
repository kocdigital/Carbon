using MassTransit.Azure.ServiceBus.Core;
using MassTransit.RabbitMqTransport;

namespace Carbon.MassTransit
{
    public class MassTransitSettings
    {
        public MassTransitBusType BusType { get; set; }
        public RabbitMqHostSettings RabbitMq { get; set; }
        public ServiceBusHostSettings AzureServiceBus { get; set; }
    }
}
