using Carbon.Test.Common.Fixtures;
using Carbon.WebApplication.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace Carbon.WebApplication.UnitTests
{
    public class IServiceCollectionExtensionsTest : IClassFixture<ConfigFixture>
    {
        private readonly ConfigFixture _configFixture;
        private readonly Mock<IConfiguration> _configurationMock;
        public IServiceCollectionExtensionsTest(ConfigFixture configFixture)
        {
            _configFixture = configFixture;
            _configurationMock = new Mock<IConfiguration>();
        }

        [Fact]
        public void AddBearerAuthentication_Successfully_ServiceCollection()
        {
            // Arrange
            var wrapper = new ServiceCollectionExtensionsWrapper();
            IServiceCollection serviceCollection = new ServiceCollection();
            var config = _configFixture.GetConfiguration("Configs/configBearerAuthentication.json");
            _configurationMock.Setup(c => c.GetSection("JwtSettings")).Returns(config.GetSection("JwtSettings"));

            // Act
            wrapper.AddBearerAuthentication(serviceCollection, _configurationMock.Object);

            // Assert      
            Assert.IsType<ServiceCollectionExtensionsWrapper>(wrapper);
        }

        [Fact]
        public void AddBearerAuthenticationToJwtSettingsNull_ThrowError_ServiceCollection()
        {
            // Arrange
            var wrapper = new ServiceCollectionExtensionsWrapper();
            IServiceCollection serviceCollection = new ServiceCollection();
            var config = _configFixture.GetConfiguration("Configs/invalidConfigBearerAuthentication.json");
            _configurationMock.Setup(c => c.GetSection("JwtSettings")).Returns(config.GetSection("JwtSettings"));

            // Assert      
            Assert.Throws<ArgumentNullException>(() => wrapper.AddBearerAuthentication(serviceCollection, _configurationMock.Object));
        }
    }
}
