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
        void AddMassTransitBus(IServiceCollection services, Action<IServiceCollectionConfigurator> configurator);
        void AddRabbitMqBus(IServiceCollectionConfigurator serviceCollection,
                                       IConfiguration configuration, Action<IServiceProvider,
                                       IRabbitMqBusFactoryConfigurator> configurator);
        void AddServiceBus(IServiceCollectionConfigurator serviceCollection,
                                       IConfiguration configuration, Action<IServiceProvider,
                                       IServiceBusBusFactoryConfigurator> configurator);
    }
}
