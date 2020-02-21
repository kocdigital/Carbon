using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Carbon.MassTransit
{
    public static class IServiceCollectionConfiguratorExtensions
    {
        private static MassTransitSettings GetMassTransitSettings(IServiceProvider provider)
        {
            var massTransitSettings = provider.GetService<IOptions<MassTransitSettings>>().Value;

            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            return massTransitSettings;
        }

        public static void UseRabbitMqBus(this IServiceCollectionConfigurator serviceCollection, Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator)
        {
            serviceCollection.AddBus(provider =>
            {
                var massTransitSettings = GetMassTransitSettings(provider);

                if (massTransitSettings.BusType == MassTransitBusType.RabbitMQ)
                {
                    if(massTransitSettings.RabbitMq == null)
                        throw new ArgumentNullException(nameof(massTransitSettings.RabbitMq));

                    return Bus.Factory.CreateUsingRabbitMq(x =>
                    {
                        x.Host(massTransitSettings.RabbitMq);

                        configurator(provider, x);
                    });
                }

                return null;
            });
        }

        public static void UseServiceBus(this IServiceCollectionConfigurator serviceCollection, Action<IServiceProvider, IServiceBusBusFactoryConfigurator> configurator)
        {
            serviceCollection.AddBus(provider =>
            {
                var massTransitSettings = GetMassTransitSettings(provider);

                if (massTransitSettings.BusType == MassTransitBusType.AzureServiceBus)
                {
                    return Bus.Factory.CreateUsingAzureServiceBus(x =>
                    {
                        var busSettings = massTransitSettings.ServiceBus;

                        x.Host(massTransitSettings.ServiceBus.ConnectionString, (c) =>
                        {
                            c.RetryLimit = busSettings.RetryLimit == 0 ? 1 : busSettings.RetryLimit;

                            if(busSettings.OperationTimeout != TimeSpan.Zero)
                                c.OperationTimeout = busSettings.OperationTimeout;
                            if (busSettings.RetryMaxBackoff != TimeSpan.Zero)
                                c.RetryMaxBackoff = busSettings.RetryMaxBackoff;
                            if (busSettings.RetryMinBackoff != TimeSpan.Zero)
                                c.RetryMinBackoff = busSettings.RetryMinBackoff;
                            if (busSettings.TokenProvider != null)
                                c.TokenProvider = busSettings.TokenProvider;

                            c.TransportType = busSettings.TransportType;
                        });

                        configurator(provider, x);
                    });
                }

                return null;
            });
        }
    }
}
