using MassTransit;
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

using System;
using System.Linq;

using MassTransitNS = MassTransit;

namespace Carbon.MassTransit.RoutingSlip
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

				serviceCollection.AddBus(cfg => busFactory(configurator, busSettings, cfg));

				serviceCollection.Collection.AddRabbitMqBusHealthCheck($"amqp://{busSettings.Username}:{busSettings.Password}@{busSettings.Host}:{busSettings.Port}{busSettings.VirtualHost}");
			}
		}
		

		private static Func<Action<IServiceProvider, IRabbitMqBusFactoryConfigurator>,
										RabbitMqSettings,
										IBusRegistrationContext,
										IBusControl> busFactory = (configurator, busSettings, provider) =>
		{
			var host = $"rabbitmq://{busSettings.Host}:{busSettings.Port}{busSettings.VirtualHost}";
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
		};

	}
}
