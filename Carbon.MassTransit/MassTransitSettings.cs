namespace Carbon.MassTransit
{
    public class MassTransitSettings
    {
        public MassTransitBusType BusType { get; set; }
        public RabbitMqSettings RabbitMq { get; set; }
        public ServiceBusSettings ServiceBus { get; set; }
    }
}
