using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using System;
using Microsoft.Extensions.Configuration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Carbon.MassTransit
{
    public static class IServiceCollectionConfiguratorExtensions
    {
        /// <summary>
        /// Mass Transit Add Extension Method
        /// </summary>
        /// <remarks>
        /// Extension method for Service Collection to add MassTransit Hosted Service as singleton and 
        /// mass transit with the given "MassTransit" configuration
        /// </remarks>
        /// <param name="services">Service Collection</param>
        /// <param name="configurator">Configuration Action</param>
        public static void AddMassTransitBus(this IServiceCollection services, Action<IServiceCollectionBusConfigurator> configurator)
        {
            services.AddSingleton<IHostedService, MassTransitHostedService>();
            services.AddMassTransit(configurator);
        }

        /// <summary>
        /// Rabbit MQ Add Extension Method
        /// </summary>
        /// <remarks>
        /// Adds new bus to the service collection  with the given configuration parameters in "MassTransit" config item.
        /// </remarks>
        /// <param name="serviceCollection">Service Collection Configurator</param>
        /// <param name="configuration">API Configuration Item with MassTransit Section</param>
        /// <param name="configurator">Service provider's Configuration Action</param>
        public static void AddRabbitMqBus(this IServiceCollectionBusConfigurator serviceCollection,
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

                var busSettings = massTransitSettings.RabbitMq;
                var host = $"rabbitmq://{busSettings.Host}:{busSettings.Port}{busSettings.VirtualHost}";

                serviceCollection.AddBus(provider =>
                {

                    return Bus.Factory.CreateUsingRabbitMq(x =>
                    {
                        x.Host(new Uri(host), (c) =>
                        {
                            if (!string.IsNullOrEmpty(busSettings.Username))
                                c.Username(busSettings.Username);
                            if (!string.IsNullOrEmpty(busSettings.Password))
                                c.Password(busSettings.Password);

                            c.PublisherConfirmation = busSettings.PublisherConfirmation;

                            if (busSettings.RequestedChannelMax > 0)
                                c.RequestedChannelMax(busSettings.RequestedChannelMax);

                            if (busSettings.RequestedConnectionTimeout > TimeSpan.Zero)
                                c.RequestedConnectionTimeout(busSettings.RequestedConnectionTimeout);

                            if (busSettings.Heartbeat > TimeSpan.Zero)
                                c.Heartbeat(busSettings.Heartbeat);

                            if (busSettings.Ssl)
                            {
                                c.UseSsl((s) =>
                                {
                                    s.UseCertificateAsAuthenticationIdentity = busSettings.UseClientCertificateAsAuthenticationIdentity;
                                    s.ServerName = busSettings.SslServerName;
                                    s.Protocol = busSettings.SslProtocol;
                                    s.Certificate = busSettings.ClientCertificate;
                                    s.CertificatePassphrase = busSettings.ClientCertificatePassphrase;
                                    s.CertificatePath = busSettings.ClientCertificatePath;
                                    s.CertificateSelectionCallback = busSettings.CertificateSelectionCallback;
                                });
                            }


                        });

                        configurator(provider, x);
                    });
                });

                //TODO: To be refactored
                //serviceCollection.Collection.AddRabbitMqBusHealthCheck(host);
            }
        }

        public static void AddRabbitMqBusHealthCheck(this IServiceCollection services,
                                       string host, HealthStatus failureStatus = HealthStatus.Unhealthy)
        {
            services.AddHealthChecks().AddRabbitMQ(host, failureStatus);
        }

        /// <summary>
        /// Azure Service Bus Add Extension Method
        /// </summary>
        /// <remarks>
        /// Adds Azure Service Bus to the service collection given with the given configuration parameters in "MassTransit" config item.
        /// </remarks>
        /// <param name="serviceCollection">Service Collection Configurator</param>
        /// <param name="configuration">API Configuration Item with MassTransit Section</param>
        /// <param name="configurator">Service provider's Configuration Action</param>
        public static void AddServiceBus(this IServiceCollectionBusConfigurator serviceCollection,
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
