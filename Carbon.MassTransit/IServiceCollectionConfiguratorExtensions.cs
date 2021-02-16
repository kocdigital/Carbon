using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using System;
using Microsoft.Extensions.Configuration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit.ExtensionsDependencyInjectionIntegration;

namespace Carbon.MassTransit
{
    public static class IServiceCollectionConfiguratorExtensions
    {
        public static void AddMassTransitBus(this IServiceCollection services, Action<IServiceCollectionBusConfigurator> configurator)
        {
            services.AddSingleton<IHostedService, MassTransitHostedService>();
            services.AddMassTransit(configurator);
        }

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

                serviceCollection.AddBus(provider =>
                {
                    var busSettings = massTransitSettings.RabbitMq;

                    return Bus.Factory.CreateUsingRabbitMq(x =>
                    {
                        x.Host(busSettings.Host, (c) =>
                        {
                            if(!string.IsNullOrEmpty(busSettings.Username))
                                c.Username(busSettings.Username);
                            if (!string.IsNullOrEmpty(busSettings.Password))
                                c.Password(busSettings.Password);

                            c.PublisherConfirmation = busSettings.PublisherConfirmation;
                           
                            if (busSettings.RequestedChannelMax > 0)
                                c.RequestedChannelMax(busSettings.RequestedChannelMax);

                            if (busSettings.RequestedConnectionTimeout > TimeSpan.Zero)
                                c.RequestedConnectionTimeout(busSettings.RequestedConnectionTimeout);

                            if(busSettings.Heartbeat > TimeSpan.Zero)
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
            }
        }

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
