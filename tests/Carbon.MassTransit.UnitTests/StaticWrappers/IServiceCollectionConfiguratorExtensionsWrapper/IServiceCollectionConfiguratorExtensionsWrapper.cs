using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.MassTransit.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper
{
    public interface IServiceCollectionConfiguratorExtensionsWrapper
    {
        void AddMassTransitBus(IServiceCollection services, Action<IServiceCollectionBusConfigurator> configurator);
        void AddRabbitMqBus(IServiceCollectionBusConfigurator serviceCollection,
                                       IConfiguration configuration, Action<IServiceProvider,
                                       IRabbitMqBusFactoryConfigurator> configurator);
        void AddServiceBus(IServiceCollectionBusConfigurator serviceCollection,
                                       IConfiguration configuration, Action<IServiceProvider,
                                       IServiceBusBusFactoryConfigurator> configurator);
    }
}
