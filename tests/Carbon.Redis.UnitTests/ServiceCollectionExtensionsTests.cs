using Carbon.Redis.UnitTests.DataShares;
using Carbon.Redis.UnitTests.Fixtures;
using Moq;
using StackExchange.Redis;
using System;
using Carbon.Redis.UnitTests.StaticWrappers.ServiceCollectionExtensions;
using Carbon.Test.Common.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace Carbon.Redis.UnitTests
{
    public class ServiceCollectionExtensionsTests : IClassFixture<RedisFixture>, IClassFixture<ConfigFixture>
    {
        #region Private Fields

        private readonly RedisFixture _redisFixture;
        private readonly ConfigFixture _configFixture;
        private readonly Mock<IServiceCollection> _serviceCollectionMock;
        private readonly Mock<IConfiguration> _configurationMock;

        #endregion

        #region Constructor

        public ServiceCollectionExtensionsTests(RedisFixture redisFixture, ConfigFixture configFixture)
        {
            _redisFixture = redisFixture;
            _configFixture = configFixture;
            _serviceCollectionMock = new Mock<IServiceCollection>();
            _configurationMock = new Mock<IConfiguration>();
        }

        #endregion

        #region AddRedisPersister

        [Theory]
        [AddRedisPersisterServicesIsNullData]
        public void AddRedisPersister_ServicesIsNull_ThrowsArgumentNullException(string expected)
        {
            ServiceCollectionExtensionsWrapper collectionExtensionsWrapper = new ServiceCollectionExtensionsWrapper();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                collectionExtensionsWrapper.AddRedisPersister(null, _configurationMock.Object));
            Assert.Contains(expected, ex.Message);
        }

        [Theory]
        [AddRedisPersisterConfigurationIsNullData]
        public void AddRedisPersister_ConfigurationIsNull_ThrowsArgumentNullException(string expected)
        {
            ServiceCollectionExtensionsWrapper collectionExtensionsWrapper = new ServiceCollectionExtensionsWrapper();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                collectionExtensionsWrapper.AddRedisPersister(_serviceCollectionMock.Object, null));
            Assert.Contains(expected, ex.Message);
        }

        [Fact]
        public void AddRedisPersister_ConnectionError_ThrowsRedisConnectionException()
        {
            ServiceCollectionExtensionsWrapper collectionExtensionsWrapper = new ServiceCollectionExtensionsWrapper();

            var config = _configFixture.GetConfiguration("Configs/config.json");
            _configurationMock.Setup(c => c.GetSection("Redis")).Returns(config.GetSection("Redis"));

            var exception = Assert.Throws<RedisConnectionException>(() =>
                collectionExtensionsWrapper.AddRedisPersister(_serviceCollectionMock.Object,
                    _configurationMock.Object));

            Assert.Equal(ConnectionFailureType.UnableToConnect, exception.FailureType);

        }

        [Fact]
        public void AddRedisPersister_ConnectionStringError_RedisException()
        {
            ServiceCollectionExtensionsWrapper collectionExtensionsWrapper = new ServiceCollectionExtensionsWrapper();

            var config = _configFixture.GetConfiguration("Configs/invalidConfig.json");
            _configurationMock.Setup(c => c.GetSection("Redis")).Returns(config.GetSection("Redis"));

            Assert.Throws<ArgumentNullException>(() =>
                collectionExtensionsWrapper.AddRedisPersister(_serviceCollectionMock.Object,
                    _configurationMock.Object));
        }
        #endregion

        #region ConvertToMD5
        [Theory]
        [ConvertToMD5Data]
        public void ConvertToMD5_ConfigurationIsNull_ThrowsArgumentNullException(string password)
        {
            ServiceCollectionExtensionsWrapper collectionExtensionsWrapper = new ServiceCollectionExtensionsWrapper();

            var result = collectionExtensionsWrapper.ConvertToMD5(password);
            Assert.NotEmpty(result);
        }
        #endregion
    }
}

