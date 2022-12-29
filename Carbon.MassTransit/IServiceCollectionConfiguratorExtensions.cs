using Carbon.MassTransit.AsyncReqResp;
using Carbon.MassTransit.AsyncReqResp.Events;
using MassTransit;
using MassTransit.AspNetCoreIntegration.HealthChecks;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus;
using MassTransit.MultiBus;
using MassTransit.RabbitMqTransport;
using MassTransit.RedisIntegration;
using MassTransit.Registration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

using MassTransitNS = MassTransit;

namespace Carbon.MassTransit
{
    /// <summary>
	/// Contains extension methods like AddRabbitMqBus, AddServiceBus for <see cref="IServiceCollectionBusConfigurator"/> and <see cref="IServiceCollection"/>
	/// </summary>
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

        #region AddRabbitMqBus

        /// <summary>
        /// Rabbit MQ Add Extension Method
        /// </summary>
        /// <remarks>
        /// Adds new bus to the service collection  with the given configuration parameters in "MassTransit" config item.
        /// </remarks>
        /// <param name="serviceCollection">Service Collection Configurator</param>
        /// <param name="configuration">API Configuration Item with MassTransit Section</param>
        /// <param name="configurator">Service provider's Configuration Action</param>
        /// <param name="healthCheckTag">Name to display in health check section</param>
        public static void AddRabbitMqBus(this IServiceCollectionBusConfigurator serviceCollection,
                                       IConfiguration configuration,
                                       Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator,
                                       string healthCheckTag = "RabbitMqConnection")
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();

            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.RabbitMQ)
            {
                serviceCollection.AddRabbitMqBus(massTransitSettings.RabbitMq, configurator, healthCheckTag);
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
        /// <param name="healthCheckTag">Name to display in health check section. If left empty name will be assigned as typeof(T).Name</param>
        public static void AddRabbitMqBus<T>(this IServiceCollectionBusConfigurator<T> serviceCollection,
                                       IConfiguration configuration,
                                       Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator,
                                       string healthCheckTag = "") where T : class, IBus
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();

            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.RabbitMQ)
            {
                serviceCollection.AddRabbitMqBus<T>(massTransitSettings.RabbitMq, configurator, healthCheckTag);
            }
        }

        /// <summary>
        /// Rabbit MQ Add Extension Method
        /// </summary>
        /// <remarks>
        /// Adds new bus to the service collection  with the given configuration parameters in "MassTransit" config item.
        /// </remarks>
        /// <param name="serviceCollection">Service Collection Configurator</param>
        /// <param name="rabbitMqSettings">RabbitMQ Settings for Host Configuration</param>
        /// <param name="configurator">Service provider's Configuration Action</param>
        /// <param name="healthCheckTag">Name to display in health check section</param>
        public static void AddRabbitMqBus(this IServiceCollectionBusConfigurator serviceCollection,
                                       RabbitMqSettings rabbitMqSettings,
                                       Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator,
                                       string healthCheckTag = "RabbitMqConnection")
        {
            if (rabbitMqSettings == null)
                throw new ArgumentNullException(nameof(rabbitMqSettings));

            serviceCollection.UsingRabbitMq((cfg, x) => rabbitMqBusFactory(configurator, rabbitMqSettings, cfg, x));

            serviceCollection.Collection.AddRabbitMqBusHealthCheck($"amqp://{rabbitMqSettings.Username}:{rabbitMqSettings.Password}@{rabbitMqSettings.Host}:{rabbitMqSettings.Port}{rabbitMqSettings.VirtualHost}", HealthStatus.Unhealthy, healthCheckTag);
        }

        /// <summary>
        /// Rabbit MQ Add Extension Method
        /// </summary>
        /// <remarks>
        /// Adds new bus to the service collection with the given configuration parameters in "MassTransit" config item.
        /// </remarks>
        /// <param name="serviceCollection">Service Collection Configurator</param>
        /// <param name="rabbitMqSettings">RabbitMQ Settings for Host Configuration</param>
        /// <param name="configurator">Service provider's Configuration Action</param>
        /// <param name="healthCheckTag">Name to display in health check section. If left empty name will be assigned as typeof(T).Name</param>
        public static void AddRabbitMqBus<T>(this IServiceCollectionBusConfigurator<T> serviceCollection,
                                       RabbitMqSettings rabbitMqSettings,
                                       Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator,
                                       string healthCheckTag = "") where T : class, IBus
        {
            if (rabbitMqSettings == null)
                throw new ArgumentNullException(nameof(rabbitMqSettings));

            serviceCollection.UsingRabbitMq((cfg, x) => rabbitMqBusFactory(configurator, rabbitMqSettings, cfg, x));

            if (string.IsNullOrEmpty(healthCheckTag))
                healthCheckTag = typeof(T).Name;

            serviceCollection.Collection.AddRabbitMqBusHealthCheck($"amqp://{rabbitMqSettings.Username}:{rabbitMqSettings.Password}@{rabbitMqSettings.Host}:{rabbitMqSettings.Port}{rabbitMqSettings.VirtualHost}", name: healthCheckTag);
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

        #endregion

        #region AddServiceBus

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
                                       IConfiguration configuration,
                                       Action<IServiceProvider, IServiceBusBusFactoryConfigurator> configurator)
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();

            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.AzureServiceBus)
            {
                serviceCollection.AddServiceBus(massTransitSettings.ServiceBus, configurator);
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
                                       IConfiguration configuration,
                                       Action<IServiceProvider, IServiceBusBusFactoryConfigurator> configurator) where T : class, IBus
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();

            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.AzureServiceBus)
            {
                serviceCollection.AddServiceBus<T>(massTransitSettings.ServiceBus, configurator);
            }
        }

        /// <summary>
        /// Azure Service Bus Add Extension Method
        /// </summary>
        /// <remarks>
        /// Adds Azure Service Bus to the service collection given with the given configuration parameters in "MassTransit" config item.
        /// </remarks>
        /// <param name="serviceCollection">Service Collection Configurator</param>
        /// <param name="serviceBusSettings">Azure Service Bus Settings for Host Configuration</param>
        /// <param name="configurator">Service provider's Configuration Action</param>
        public static void AddServiceBus(this IServiceCollectionBusConfigurator serviceCollection,
                                       ServiceBusSettings serviceBusSettings,
                                       Action<IServiceProvider, IServiceBusBusFactoryConfigurator> configurator)
        {
            if (serviceBusSettings == null)
                throw new ArgumentNullException(nameof(serviceBusSettings));

            serviceCollection.AddBus(cfg => serviceBusFactory(configurator, serviceBusSettings, cfg));
        }

        /// <summary>
        /// Azure Service Bus Add Extension Method
        /// </summary>
        /// <remarks>
        /// Adds Azure Service Bus to the service collection given with the given configuration parameters in "MassTransit" config item.
        /// </remarks>
        /// <param name="serviceCollection">Service Collection Configurator</param>
        /// <param name="serviceBusSettings">Azure Service Bus Settings for Host Configuration</param>
        /// <param name="configurator">Service provider's Configuration Action</param>
        public static void AddServiceBus<T>(this IServiceCollectionBusConfigurator<T> serviceCollection,
                                       ServiceBusSettings serviceBusSettings,
                                       Action<IServiceProvider, IServiceBusBusFactoryConfigurator> configurator) where T : class, IBus
        {
            if (serviceBusSettings == null)
                throw new ArgumentNullException(nameof(serviceBusSettings));

            serviceCollection.AddBus(cfg => serviceBusFactory(configurator, serviceBusSettings, cfg));
        }

        #endregion

        /// <summary>
        /// Adds AsyncRequestResponsePattern. Use this overload for only requestor
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddAsyncRequestResponsePatternForRequestor<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class, IConsumer<IResponseCarrier>
        {
            var apiname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            services.AddMassTransitBus<IReqRespRequestorBus>(cfg =>
            {
                cfg.AddConsumer<T>();

                cfg.AddRequestClient<IRequestStarterRequest>();

                cfg.AddSagaStateMachine<RequestResponseStateMachine, RequestResponseState>()
                .RedisRepository(x =>
                {
                    x.ConnectionFactory(k => (ConnectionMultiplexer)k.GetRequiredService<IConnectionMultiplexer>());
                    x.KeyPrefix = "exdata";
                    x.Expiry = new System.TimeSpan(0, 30, 0);
                    x.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    x.LockRetryTimeout = new System.TimeSpan(0, 0, 2);
                    x.LockTimeout = new System.TimeSpan(0, 0, 30);
                }
                );

                cfg.AddRabbitMqBus(configuration, (provider, busFactoryConfig) =>
                {
                    busFactoryConfig.ReceiveEndpoint(apiname + "-request-starter-state", e =>
                    {
                        e.AddAsHighAvailableQueue(configuration);
                        e.ConfigureSaga<RequestResponseState>((IBusRegistrationContext)provider);
                    });

                    busFactoryConfig.Publish<IRequestStarterRequest>(x => { });

                    busFactoryConfig.ReceiveEndpoint(apiname + "-Req.Resp.Async-RespHandler", configurator =>
                    {
                        configurator.AddAsHighAvailableQueue(configuration);
                        configurator.Consumer<T>(provider);
                    });
                });

                cfg.AddServiceBus(configuration, (provider, busFactoryConfig) =>
                {
                    busFactoryConfig.ReceiveEndpoint(apiname + "-request-starter-state", e =>
                    {
                        e.ConfigureSaga<RequestResponseState>((IBusRegistrationContext)provider);
                    });

                    busFactoryConfig.Publish<IRequestStarterRequest>(x => { });

                    busFactoryConfig.ReceiveEndpoint(apiname + "-Req.Resp.Async-RespHandler", configurator =>
                    {
                        configurator.Consumer<T>(provider);
                    });
                });
            });
        }

        /// <summary>
        /// Adds AsyncRequestResponsePattern. Use this overload for only responder to requestor
        /// </summary>
        /// <typeparam name="T">Your request handler consumer where you respond to consumer</typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="responseDestinationPath"></param>
        public static void AddAsyncRequestResponsePatternForResponder<T>(this IServiceCollection services, IConfiguration configuration, string responseDestinationPath)
            where T : class, IConsumer<IRequestCarrierRequest>
        {
            if (String.IsNullOrEmpty(responseDestinationPath))
            {
                throw new Exception("responseDestinationPath cannot be null or empty");
            }

            services.AddMassTransitBus<IReqRespResponderBus>(cfg =>
            {
                cfg.AddConsumer<T>();

                cfg.AddRabbitMqBus(configuration, (provider, busFactoryConfig) =>
                {

                    busFactoryConfig.ReceiveEndpoint("Req.Resp.Async-" + responseDestinationPath, configurator =>
                    {
                        configurator.AddAsHighAvailableQueue(configuration);
                        configurator.Consumer<T>(provider);
                    });

                });

                cfg.AddServiceBus(configuration, (provider, busFactoryConfig) =>
                {
                    busFactoryConfig.ReceiveEndpoint("Req.Resp.Async-" + responseDestinationPath, configurator =>
                    {
                        configurator.Consumer<T>(provider);
                    });
                });
            });
        }

        /// <summary>
        /// Adds AsyncRequestResponsePattern. Use this overload for only responder to requestor
        /// </summary>
        /// <typeparam name="T">Your request handler consumer where you respond to consumer</typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="responseDestinationPath"></param>
        public static void AddAsyncRequestResponsePatternForResponder(this IServiceCollection services, IConfiguration configuration, Dictionary<string, Type> responseDestinationPaths)
        {
            if (responseDestinationPaths == null || responseDestinationPaths.Count == 0)
            {
                throw new Exception("responseDestinationPath cannot be null or empty");
            }
            services.AddMassTransitBus<IReqRespResponderBus>(cfg =>
            {
                foreach (var respPath in responseDestinationPaths)
                {
                    if (!(typeof(IConsumer<IRequestCarrierRequest>).IsAssignableFrom(respPath.Value)))
                    {
                        throw new Exception("Consumer Type must be IConsumer<IRequestCarrierRequest>");
                    }

                    cfg.AddConsumer(respPath.Value);
                }
                cfg.AddRabbitMqBus(configuration, (provider, busFactoryConfig) =>
                {
                    foreach (var responseDestinationPath in responseDestinationPaths)
                    {
                        var consumerType = responseDestinationPath.Value;
                        busFactoryConfig.ReceiveEndpoint("Req.Resp.Async-" + responseDestinationPath.Key, configurator =>
                        {
                            configurator.AddAsHighAvailableQueue(configuration);
                            configurator.ConfigureConsumer((IRegistration)provider, consumerType);
                        });
                    }
                });

                cfg.AddServiceBus(configuration, (provider, busFactoryConfig) =>
                {
                    foreach (var responseDestinationPath in responseDestinationPaths)
                    {
                        var consumerType = responseDestinationPath.Value;
                        busFactoryConfig.ReceiveEndpoint("Req.Resp.Async-" + responseDestinationPath, configurator =>
                        {
                            configurator.ConfigureConsumer((IRegistration)provider, consumerType);
                        });
                    }
                });
            });
        }


        /// <summary>
        /// ReceiveEndpoint queue will be declared as a quorum queue, if it is already declared as default, it will delete the existing one and create new HA queue
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="configuration">Configuration</param>
        /// <remarks>Currently only works with RabbitMQ environments.</remarks>
        public static void AddAsHighAvailableQueue(this IRabbitMqReceiveEndpointConfigurator cfg,
                                      IConfiguration configuration)
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();
            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.RabbitMQ)
            {
                cfg.AddAsHighAvailableQueue(massTransitSettings.RabbitMq);
            }
            else
            {
                throw new NotImplementedException();
            }
            
        }

        /// <summary>
        /// ReceiveEndpoint queue will be declared as a quorum queue, if it is already declared as default, it will delete the existing one and create new HA queue
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="rabbitMqSettings">RabbitMQ Connection Settings</param>
        public static void AddAsHighAvailableQueue(this IRabbitMqReceiveEndpointConfigurator cfg,
                                      RabbitMqSettings rabbitMqSettings)
        {
            cfg.SetQuorumQueue(3);
            cfg.ConnectReceiveEndpointObserver(new RabbitMQReceiveEndpointObserver(rabbitMqSettings));
        }

        /// <summary>
        /// ReceiveEndpoint queue will be declared as a classic queue, if it is already declared as quorum or something else, it will delete the existing one and create new classic queue
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="configuration">Configuration</param>
        /// <remarks>Currently only works with RabbitMQ environments.</remarks>
        public static void AddAsDefaultQueue(this IReceiveEndpointConfigurator cfg,
                                      IConfiguration configuration)
        {
            var massTransitSettings = configuration.GetSection("MassTransit").Get<MassTransitSettings>();
            if (massTransitSettings == null)
                throw new ArgumentNullException(nameof(massTransitSettings));

            if (massTransitSettings.BusType == MassTransitBusType.RabbitMQ)
            {
                cfg.AddAsDefaultQueue(massTransitSettings.RabbitMq);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// ReceiveEndpoint queue will be declared as a classic queue, if it is already declared as quorum or something else, it will delete the existing one and create new classic queue
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="rabbitMqSettings">RabbitMQ connection settings</param>
        public static void AddAsDefaultQueue(this IReceiveEndpointConfigurator cfg,
                                      RabbitMqSettings rabbitMqSettings)
        {
            cfg.ConnectReceiveEndpointObserver(new RabbitMQReceiveEndpointObserver(rabbitMqSettings));
        }

        private static Func<Action<IServiceProvider, IServiceBusBusFactoryConfigurator>,
                                        ServiceBusSettings,
                                        IBusRegistrationContext,
                                        IBusControl> serviceBusFactory = (configurator, busSettings, provider) =>
        {
            return Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                x.Host(busSettings.ConnectionString, (c) =>
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
                                        IRabbitMqBusFactoryConfigurator> rabbitMqBusFactory = (configurator, busSettings, provider, x) =>
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
