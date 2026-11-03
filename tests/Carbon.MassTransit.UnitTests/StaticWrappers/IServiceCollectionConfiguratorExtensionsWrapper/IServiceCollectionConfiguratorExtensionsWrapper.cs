using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.MassTransit.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper
{
    public interface IServiceCollectionConfiguratorExtensionsWrapper
    {
        void AddMassTransitBus(IServiceCollection services, Action<IBusRegistrationConfigurator> configurator);
        void AddMassTransitBus<T>(IServiceCollection services, Action<IBusRegistrationConfigurator<T>> configurator) where T : class, IBus;
        void AddRabbitMqBus(IBusRegistrationConfigurator serviceCollection,
                                        IConfiguration configuration, Action<IServiceProvider,
                                        IRabbitMqBusFactoryConfigurator> configurator);
        void AddRabbitMqBus<T>(IBusRegistrationConfigurator<T> serviceCollection,
                                        IConfiguration configuration,
                                        Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator) where T : class, IBus;
        void AddServiceBus(IBusRegistrationConfigurator serviceCollection,
                                        IConfiguration configuration, Action<IServiceProvider,
                                        IServiceBusBusFactoryConfigurator> configurator);
    }
}
