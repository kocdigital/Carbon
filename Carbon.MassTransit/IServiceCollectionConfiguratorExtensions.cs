using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using System;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Carbon.MassTransit
{
    public static class IServiceCollectionConfiguratorExtensions
    {
        public static void AddMassTransitBus(this IServiceCollection services, Action<IServiceCollectionConfigurator> configurator)
        {
            services.AddSingleton<IHostedService, MassTransitHostedService>();
            services.AddMassTransit(configurator);
        }

        public static void AddBus(this IServiceCollectionConfigurator serviceCollection, 
                                       IConfiguration configuration, Action<IServiceProvider, 
                                       IRabbitMqBusFactoryConfigurator> configurator)
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();

            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.RabbitMQ)
            {
                if (massTransitSettings.RabbitMq == null)
                    throw new ArgumentNullException(nameof(massTransitSettings.RabbitMq));

                serviceCollection.AddBus(provider =>
                {
                    return Bus.Factory.CreateUsingRabbitMq(x =>
                    {
                        x.Host(massTransitSettings.RabbitMq);

                        configurator(provider, x);
                    });
                });
            }
        }

        public static void AddBus(this IServiceCollectionConfigurator serviceCollection, 
                                       IConfiguration configuration, Action<IServiceProvider, 
                                       IServiceBusBusFactoryConfigurator> configurator)
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();

            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.AzureServiceBus)
            {
                if (massTransitSettings.ServiceBus == null)
                    throw new ArgumentNullException(nameof(massTransitSettings.ServiceBus));

                serviceCollection.AddBus(provider =>
                {
                    return Bus.Factory.CreateUsingAzureServiceBus(x =>
                    {
                        var busSettings = massTransitSettings.ServiceBus;

                        x.Host(massTransitSettings.ServiceBus.ConnectionString, (c) =>
                        {
                            c.RetryLimit = busSettings.RetryLimit == 0 ? 1 : busSettings.RetryLimit;

                            if (busSettings.OperationTimeout != TimeSpan.Zero)
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
                });
            }
        }
    }
}
