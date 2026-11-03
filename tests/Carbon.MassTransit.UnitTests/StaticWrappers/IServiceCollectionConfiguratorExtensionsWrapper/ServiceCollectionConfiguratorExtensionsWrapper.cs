using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.MassTransit.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper
{
    public class ServiceCollectionConfiguratorExtensionsWrapper : IServiceCollectionConfiguratorExtensionsWrapper
    {
        public void AddMassTransitBus(IServiceCollection services, Action<IBusRegistrationConfigurator> configurator)
        {
            IServiceCollectionConfiguratorExtensions.AddMassTransitBus(services, configurator);
        }

        public void AddMassTransitBus<T>(IServiceCollection services, Action<IBusRegistrationConfigurator<T>> configurator) where T: class, IBus
        {
            IServiceCollectionConfiguratorExtensions.AddMassTransitBus<T>(services, configurator);
        }

        public void AddRabbitMqBus(IBusRegistrationConfigurator serviceCollection, IConfiguration configuration, Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator)
        {
            IServiceCollectionConfiguratorExtensions.AddRabbitMqBus(serviceCollection, configuration, configurator);
        }

        public void AddRabbitMqBus<T>(IBusRegistrationConfigurator<T> serviceCollection, IConfiguration configuration, Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator) where T : class, IBus
        {
            IServiceCollectionConfiguratorExtensions.AddRabbitMqBus<T>(serviceCollection, configuration, configurator);
        }

        public void AddServiceBus(IBusRegistrationConfigurator serviceCollection, IConfiguration configuration, Action<IServiceProvider, IServiceBusBusFactoryConfigurator> configurator)
        {
            IServiceCollectionConfiguratorExtensions.AddServiceBus(serviceCollection, configuration, configurator);
        }
    }
}
