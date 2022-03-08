using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.MassTransit.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper
{
    public class ServiceCollectionConfiguratorExtensionsWrapper : IServiceCollectionConfiguratorExtensionsWrapper
    {
        public void AddMassTransitBus(IServiceCollection services, Action<IServiceCollectionBusConfigurator> configurator)
        {
            IServiceCollectionConfiguratorExtensions.AddMassTransitBus(services, configurator);
        }

        public void AddMassTransitBus<T>(IServiceCollection services, Action<IServiceCollectionConfigurator<T>> configurator) where T: class, IBus
        {
            IServiceCollectionConfiguratorExtensions.AddMassTransitBus<T>(services, configurator);
        }

        public void AddRabbitMqBus(IServiceCollectionBusConfigurator serviceCollection, IConfiguration configuration, Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator)
        {
            IServiceCollectionConfiguratorExtensions.AddRabbitMqBus(serviceCollection, configuration, configurator);
        }

        public void AddRabbitMqBus<T>(IServiceCollectionConfigurator<T> serviceCollection, IConfiguration configuration, Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator, string busName = null) where T : class, IBus
        {
            IServiceCollectionConfiguratorExtensions.AddRabbitMqBus<T>(serviceCollection, configuration, configurator, busName);
        }

        public void AddServiceBus(IServiceCollectionBusConfigurator serviceCollection, IConfiguration configuration, Action<IServiceProvider, IServiceBusBusFactoryConfigurator> configurator)
        {
            IServiceCollectionConfiguratorExtensions.AddServiceBus(serviceCollection, configuration, configurator);
        }
    }
}
