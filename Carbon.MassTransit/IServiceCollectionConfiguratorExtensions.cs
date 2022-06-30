using MassTransit;
using MassTransit.AspNetCoreIntegration.HealthChecks;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus;
using MassTransit.MultiBus;
using MassTransit.RabbitMqTransport;
using MassTransit.Registration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

using MassTransitNS = MassTransit;

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
        public static void AddMassTransitBus(this IServiceCollection services, Action<IServiceCollectionBusConfigurator> configurator, bool useDefaultHostedService = true)
        {
            services.AddMassTransit(configurator);
            if (useDefaultHostedService)
                services.AddMassTransitHostedService();
            else
            {
                services.AddSingleton<IConfigureOptions<HealthCheckServiceOptions>>(provider =>
                new ConfigureBusHealthCheckServiceOptions(provider.GetServices<IBusInstance>(), new[] { "ready", "masstransit" }));
                services.AddSingleton<IHostedService, MassTransitHostedService>();
            }
        }

        /// <summary>
        /// Adds extra MassTransit Bus
        /// </summary>
        /// <remarks>
        /// Extension method for Service Collection to add extra MassTransit Bus for given Interface with the given MultiBus configuration.
        /// </remarks>
        /// <param name="services">Service Collection</param>
        /// <param name="configurator">Configuration Action</param>
        public static void AddMassTransitBus<T>(this IServiceCollection services, Action<IServiceCollectionBusConfigurator<T>> configurator) where T : class, IBus
        {
            services.AddMassTransit<T>(configurator);

            //HACK: Hosted service for consumers, contains bus depot
            //BusDepot is registered as single instance and it handles multiple IBus instances so only one hosted service is enough
            //NOTE: Composed from Masstransit source code v7.1.5, so watch out while upgrading masstansit
            if (!services.Any(descriptor => descriptor?.ImplementationFactory?.Method?.ReturnType == typeof(MassTransitNS.AspNetCoreIntegration.MassTransitHostedService)))
            {
                MassTransitNS.AspNetCoreIntegration.MassTransitHostedService HostedServiceFactory(IServiceProvider provider)
                {
                    var busDepot = provider.GetRequiredService<IBusDepot>();
                    return new MassTransitNS.AspNetCoreIntegration.MassTransitHostedService(busDepot, false);
                }
                services.AddHostedService(HostedServiceFactory);
            }
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
                serviceCollection.UsingRabbitMq((cfg, x) => busFactory(configurator, busSettings, cfg, x));

                serviceCollection.Collection.AddRabbitMqBusHealthCheck($"amqp://{busSettings.Username}:{busSettings.Password}@{busSettings.Host}:{busSettings.Port}{busSettings.VirtualHost}", HealthStatus.Unhealthy, "RabbitMqConnection");
            }
        }

        /// <summary>
        /// Rabbit MQ Add Extension Method
        /// </summary>
        /// <remarks>
        /// Adds new bus to the service collection with the given configuration parameters in "MassTransit" config item.
        /// </remarks>
        /// <param name="serviceCollection">Service Collection Configurator</param>
        /// <param name="configuration">API Configuration Item with MassTransit Section</param>
        /// <param name="configurator">Service provider's Configuration Action</param>
        public static void AddRabbitMqBus<T>(this IServiceCollectionBusConfigurator<T> serviceCollection,
                                       IConfiguration configuration, Action<IServiceProvider,
                                       IRabbitMqBusFactoryConfigurator> configurator) where T : class, IBus
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();

            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.RabbitMQ)
            {
                if (massTransitSettings.RabbitMq == null)
                    throw new ArgumentNullException(nameof(massTransitSettings.RabbitMq));

                var busSettings = massTransitSettings.RabbitMq;

                serviceCollection.UsingRabbitMq((cfg, x) =>
                {
                    busFactory(configurator, busSettings, cfg, x);
                });

                serviceCollection.Collection.AddRabbitMqBusHealthCheck($"amqp://{busSettings.Username}:{busSettings.Password}@{busSettings.Host}:{busSettings.Port}{busSettings.VirtualHost}", name: typeof(T).Name);
            }
        }

        public static void AddRabbitMqBusHealthCheck(this IServiceCollection services,
                                       string host, HealthStatus failureStatus = HealthStatus.Unhealthy, string name = null)
        {
            services.AddHealthChecks().AddRabbitMQ(sp =>
            {
                var factory = new RabbitMQ.Client.ConnectionFactory()
                {
                    Uri = new Uri(host)
                };
                return factory.CreateConnection();
            }, name: name, failureStatus: failureStatus);
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

                serviceCollection.AddBus(cfg => serviceBusFactory(configurator, massTransitSettings, cfg));
            }
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
        public static void AddServiceBus<T>(this IServiceCollectionBusConfigurator<T> serviceCollection,
                                       IConfiguration configuration, Action<IServiceProvider,
                                       IServiceBusBusFactoryConfigurator> configurator) where T : class, IBus
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();

            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.AzureServiceBus)
            {
                if (massTransitSettings.ServiceBus == null)
                    throw new ArgumentNullException(nameof(massTransitSettings.ServiceBus));

                serviceCollection.AddBus(cfg => serviceBusFactory(configurator, massTransitSettings, cfg));
            }
        }

        private static Func<Action<IServiceProvider, IServiceBusBusFactoryConfigurator>,
                                        MassTransitSettings,
                                        IBusRegistrationContext,
                                        IBusControl> serviceBusFactory = (configurator, massTransitSettings, provider) =>
        {
            return Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                var busSettings = massTransitSettings.ServiceBus;

                x.Host(massTransitSettings.ServiceBus.ConnectionString, (c) =>
                {
                    c.RetryLimit = busSettings.RetryLimit == 0 ? 1 : busSettings.RetryLimit;


                    if (busSettings.RetryMaxBackoff != TimeSpan.Zero)
                        c.RetryMaxBackoff = busSettings.RetryMaxBackoff;
                    if (busSettings.RetryMinBackoff != TimeSpan.Zero)
                        c.RetryMinBackoff = busSettings.RetryMinBackoff;

                    c.TransportType = busSettings.TransportType;
                });

                configurator(provider, x);
            });
        };

        private static Func<Action<IServiceProvider, IRabbitMqBusFactoryConfigurator>,
                                        RabbitMqSettings,
                                        IBusRegistrationContext,
                                        IRabbitMqBusFactoryConfigurator,
                                        IRabbitMqBusFactoryConfigurator> busFactory = (configurator, busSettings, provider, x) =>
        {
            var host = $"rabbitmq://{busSettings.Host}:{busSettings.Port}{busSettings.VirtualHost}";
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
                configurator(provider, x);
            });
            return x;
        };

    }
}
