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
    public interface IServiceCollectionConfiguratorExtensionsWrapper
    {
        void AddMassTransitBus(IServiceCollection services, Action<IServiceCollectionBusConfigurator> configurator);
        void AddMassTransitBus<T>(IServiceCollection services, Action<IServiceCollectionConfigurator<T>> configurator) where T : class, IBus;
        void AddRabbitMqBus(IServiceCollectionBusConfigurator serviceCollection,
                                        IConfiguration configuration, Action<IServiceProvider,
                                        IRabbitMqBusFactoryConfigurator> configurator);
        void AddRabbitMqBus<T>(IServiceCollectionConfigurator<T> serviceCollection,
                                        IConfiguration configuration,
                                        Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator,
                                        string busName = null) where T : class, IBus;
        void AddServiceBus(IServiceCollectionBusConfigurator serviceCollection,
                                        IConfiguration configuration, Action<IServiceProvider,
                                        IServiceBusBusFactoryConfigurator> configurator);
    }
}
