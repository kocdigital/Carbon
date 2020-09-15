namespace Carbon.MassTransit
{
    public class MassTransitSettings
    {
        /// <summary>
        /// Mass Transit Bus Type
        /// </summary>
        /// <remarks>
        /// Rabit MQ or Azure Service bus can be selected.
        /// </remarks>
        public MassTransitBusType BusType { get; set; }

        /// <summary>
        /// Rabbit Mq bus settings
        /// </summary>
        /// <remarks>
        /// Should be loaded according to the selected BusType
        /// </remarks>
        public RabbitMqSettings RabbitMq { get; set; }

        /// <summary>
        /// Azure Service Bus settings
        /// </summary>
        /// <remarks>
        /// Should be loaded according to the selected BusType
        /// </remarks>
        public ServiceBusSettings ServiceBus { get; set; }
    }
}
