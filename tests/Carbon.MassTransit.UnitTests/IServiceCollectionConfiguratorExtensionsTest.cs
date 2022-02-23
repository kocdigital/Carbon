using Carbon.MassTransit.UnitTests.StaticWrappers.IBusWrapper;
using Carbon.MassTransit.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper;
using Carbon.Test.Common.Fixtures;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using Xunit;

namespace Carbon.MassTransit.UnitTests
{
    public class IServiceCollectionConfiguratorExtensionsTest : IClassFixture<ConfigFixture>
    {
        private readonly Mock<IServiceCollection> _serviceCollectionMock;
        private readonly Mock<Action<IServiceCollectionBusConfigurator>> _serviceCollectionConfiguratorMock;
        private readonly Mock<Action<IServiceCollectionConfigurator<ITestBus>>> _serviceCollectionConfiguratorGenericMock;
        private readonly Mock<IServiceCollectionBusConfigurator> _configuratorMock;
        private readonly Mock<IServiceCollectionConfigurator<ITestBus>> _configuratorMockGeneric;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<Action<IServiceProvider, IRabbitMqBusFactoryConfigurator>> _actionRabbitMqBusFactoryConfiguratorMock;
        private readonly Mock<Action<IServiceProvider, IServiceBusBusFactoryConfigurator>> _actionServiceBusBusFactoryConfiguratorMock;
        private readonly ConfigFixture _configFixture;
        public IServiceCollectionConfiguratorExtensionsTest(ConfigFixture configFixture)
        {
            _serviceCollectionMock = new Mock<IServiceCollection>();
            _serviceCollectionConfiguratorMock = new Mock<Action<IServiceCollectionBusConfigurator>>();
            _serviceCollectionConfiguratorGenericMock = new Mock<Action<IServiceCollectionConfigurator<ITestBus>>>();
            _configurationMock = new Mock<IConfiguration>();
            _actionRabbitMqBusFactoryConfiguratorMock = new Mock<Action<IServiceProvider, IRabbitMqBusFactoryConfigurator>>();
            _actionServiceBusBusFactoryConfiguratorMock = new Mock<Action<IServiceProvider, IServiceBusBusFactoryConfigurator>>();
            _configuratorMock = new Mock<IServiceCollectionBusConfigurator>();
            _configuratorMockGeneric = new Mock<IServiceCollectionConfigurator<ITestBus>>();
            _configFixture = configFixture;
        }


        [Fact]
        public void AddMassTransitBusTo_Successfully_ServiceCollection()
        {
            // Arrange
            // Act

            var serviceCollectionWrapper = new ServiceCollectionConfiguratorExtensionsWrapper();
            serviceCollectionWrapper.AddMassTransitBus(_serviceCollectionMock.Object, _serviceCollectionConfiguratorMock.Object);

            // Assert      
            _serviceCollectionMock.Verify(serviceCollection => serviceCollection.Add(
                    It.Is<ServiceDescriptor>(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IHostedService)
                    && serviceDescriptor.ImplementationType == typeof(MassTransitHostedService) && serviceDescriptor.Lifetime == ServiceLifetime.Singleton)));
        }

        [Fact]
        public void AddMassTransitBusGenericTo_Successfully_ServiceCollection()
        {
            // Arrange
            // Act

            var serviceCollectionWrapper = new ServiceCollectionConfiguratorExtensionsWrapper();
            serviceCollectionWrapper.AddMassTransitBus<ITestBus>(_serviceCollectionMock.Object, _serviceCollectionConfiguratorGenericMock.Object);

            // Assert      
            _serviceCollectionMock.Verify(serviceCollection => serviceCollection.Add(
                    It.Is<ServiceDescriptor>(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IHostedService)
                    && serviceDescriptor.ImplementationType == typeof(MassTransitHostedService) && serviceDescriptor.Lifetime == ServiceLifetime.Singleton)));
        }

        [Fact]
        public void AddAddRabbitMqBusTo_Successfully_ServiceCollection()
        {
            // Arrange
            var config = _configFixture.GetConfiguration("Configs/config.json");
            _configurationMock.Setup(c => c.GetSection("MassTransit")).Returns(config.GetSection("MassTransit"));
            // Act
            var serviceCollectionWrapper = new ServiceCollectionConfiguratorExtensionsWrapper();
            var exception =  Record.Exception(() => serviceCollectionWrapper.AddRabbitMqBus(_configuratorMock.Object, _configurationMock.Object, _actionRabbitMqBusFactoryConfiguratorMock.Object));
            // Assert      
            Assert.Null(exception);
        }

        [Fact]
        public void AddAddRabbitMqBusGenericTo_Successfully_ServiceCollection()
        {
            // Arrange
            var config = _configFixture.GetConfiguration("Configs/config.json");
            _configurationMock.Setup(c => c.GetSection("MassTransit")).Returns(config.GetSection("MassTransit"));
            // Act
            var serviceCollectionWrapper = new ServiceCollectionConfiguratorExtensionsWrapper();
            var exception = Record.Exception(() => serviceCollectionWrapper.AddRabbitMqBus<ITestBus>(_configuratorMockGeneric.Object, _configurationMock.Object, _actionRabbitMqBusFactoryConfiguratorMock.Object,"testBus"));
            // Assert      
            Assert.Null(exception);
        }

        [Fact]
        public void AddAddRabbitMqBusTo_ThrowError_ServiceCollection()
        {
            // Arrange
            var config = _configFixture.GetConfiguration("Configs/config.json");
            // Act
            var httpStatus = new ServiceCollectionConfiguratorExtensionsWrapper();
            // Assert 
            Assert.Throws<ArgumentNullException>(() => httpStatus.AddRabbitMqBus(_configuratorMock.Object, _configurationMock.Object, _actionRabbitMqBusFactoryConfiguratorMock.Object));
        }
        [Fact]
        public void AddAddRabbitMqBusToRabbitMQNull_ThrowError_ServiceCollection()
        {
            // Arrange
            var config = _configFixture.GetConfiguration("Configs/invalidRabitMqconfig.json");
            _configurationMock.Setup(c => c.GetSection("MassTransit")).Returns(config.GetSection("MassTransit"));
            // Act
            var httpStatus = new ServiceCollectionConfiguratorExtensionsWrapper();
            // Assert 
            Assert.Throws<ArgumentNullException>(() => httpStatus.AddRabbitMqBus(_configuratorMock.Object, _configurationMock.Object, _actionRabbitMqBusFactoryConfiguratorMock.Object));
        }

        [Fact]
        public void AddServiceBus_Successfully_ServiceCollection()
        {
            // Arrange
            var config = _configFixture.GetConfiguration("Configs/configServiceBus.json");
            _configurationMock.Setup(c => c.GetSection("MassTransit")).Returns(config.GetSection("MassTransit"));
            // Act
            var httpStatus = new ServiceCollectionConfiguratorExtensionsWrapper();
            var exception = Record.Exception(() => httpStatus.AddServiceBus(_configuratorMock.Object, _configurationMock.Object, _actionServiceBusBusFactoryConfiguratorMock.Object));
            // Assert      
            Assert.Null(exception);
        }
        [Fact]
        public void AddServiceBus_ThrowError_ServiceCollection()
        {
            // Arrange
            // Act
            var httpStatus = new ServiceCollectionConfiguratorExtensionsWrapper();
            // Assert 
            Assert.Throws<ArgumentNullException>(() => httpStatus.AddServiceBus(_configuratorMock.Object, _configurationMock.Object, _actionServiceBusBusFactoryConfiguratorMock.Object));
        }
        [Fact]
        public void AddServiceBusRabbitMQNull_ThrowError_ServiceCollection()
        {
            // Arrange
            var config = _configFixture.GetConfiguration("Configs/invalidServiceBusConfig.json");
            _configurationMock.Setup(c => c.GetSection("MassTransit")).Returns(config.GetSection("MassTransit"));
            // Act
            var httpStatus = new ServiceCollectionConfiguratorExtensionsWrapper();
            // Assert 
            Assert.Throws<ArgumentNullException>(() => httpStatus.AddServiceBus(_configuratorMock.Object, _configurationMock.Object, _actionServiceBusBusFactoryConfiguratorMock.Object));
        }
    }
}
