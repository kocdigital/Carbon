using Carbon.Test.Common.Fixtures;
using Carbon.WebApplication.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Carbon.WebApplication.UnitTests
{
    public class IServiceCollectionExtensionsTest : IClassFixture<ConfigFixture>
    {
        private readonly ConfigFixture _configFixture;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IServiceCollection> _serviceCollectionMock;
        public IServiceCollectionExtensionsTest(ConfigFixture configFixture)
        {
            _configFixture = configFixture;
            _configurationMock = new Mock<IConfiguration>();
            _serviceCollectionMock = new Mock<IServiceCollection>();
        }

        [Fact]
        public void AddBearerAuthentication_Successfully_ServiceCollection()
        {
            // Arrange
            var config = _configFixture.GetConfiguration("Configs/configBearerAuthentication.json");
            _configurationMock.Setup(c => c.GetSection("JwtSettings")).Returns(config.GetSection("JwtSettings"));
            // Act

            //TODO:
            _serviceCollectionMock.Setup(c => c.AddAuthentication()).Returns(new Microsoft.AspNetCore.Authentication.AuthenticationBuilder(_serviceCollectionMock.Object));
            ServiceCollectionExtensions.AddBearerAuthentication(_serviceCollectionMock.Object, _configurationMock.Object);

            var httpStatus = new ServiceCollectionExtensionsWrapper();
            var exception = Record.Exception(() => httpStatus.AddBearerAuthentication(_serviceCollectionMock.Object, _configurationMock.Object));
            // Assert      
            Assert.Null(exception);            
        }
    }
}
