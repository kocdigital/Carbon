using Carbon.Redis.UnitTests.DataShares;
using Carbon.Redis.UnitTests.Fixtures;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace Carbon.Redis.UnitTests
{
    public class RedisExtensionsTests : IClassFixture<RedisFixture>
    {
        #region Private Fields
        private readonly RedisFixture _redisExtensionFixture;
        private readonly Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;
        #endregion

        #region Constructor
        public RedisExtensionsTests(RedisFixture redisExtensionFixture)
        {
            _redisExtensionFixture = redisExtensionFixture;
            _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        }
        #endregion

        #region RemoveKeysByPattern
        [Theory]
        [SendKeyValidData]
        public async Task RemoveKeysByPattern_RedisDisabled_RedisDisabledResult(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabaseWithoutMultiplexer.RemoveKeysByPattern(key, _redisExtensionFixture.Multiplexer);

            Assert.IsType<ValueTuple<List<string>, List<string>, string>>(result);
            Assert.NotNull(result.ToTuple().Item3);
            Assert.Equal(result.ToTuple().Item3, RedisConstants.RedisDisabled);
        }

        [Theory]
        [SendKeyValidData]
        public async Task RemoveKeysByPattern_Successful_KeyRemovedResult(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabase.RemoveKeysByPattern(key, _redisExtensionFixture.RedisServer.Multiplexer);

            Assert.IsType<ValueTuple<List<string>, List<string>, string>>(result);
            Assert.Single(result.ToTuple().Item1);
            Assert.Empty(result.ToTuple().Item2);
            Assert.Null(result.ToTuple().Item3);
        }

        //[Theory]
        //[SendKeyValidData]
        //public async Task RemoveKeysByPattern_CouldntRemoveKey_KeyNotRemovedResult(string key)
        //{
        //    var result = await _redisExtensionFixture.RedisDatabase.RemoveKeysByPattern(key, _redisExtensionFixture.RedisServer.Multiplexer);

        //    Assert.IsType<ValueTuple<List<string>, List<string>, string>>(result);
        //    Assert.Empty(result.ToTuple().Item1);
        //    Assert.Single(result.ToTuple().Item2);
        //    Assert.Null(result.ToTuple().Item3);
        //}

        [Theory]
        [SendKeyValidData]
        public async Task RemoveKeysByPattern_GetsException_ReturnExceptionalResult(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabase.RemoveKeysByPattern(key, _mockConnectionMultiplexer.Object);

            Assert.IsType<ValueTuple<List<string>, List<string>, string>>(result);
            Assert.Null(result.ToTuple().Item1);
            Assert.Null(result.ToTuple().Item2);
            Assert.NotEmpty(result.ToTuple().Item3);
        }
        #endregion

        #region Get
        [Theory]
        [SendWrongSyntaxedKeyInvalidData]
        public async Task Get_KeyIsNotValid_KeyIsNotValidResult(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabase.Get<object>(key);

            Assert.IsType<ValueTuple<object, string>>(result);
            Assert.Null(result.ToTuple().Item1);
        }

        [Theory]
        [SendKeyValidData]
        public async Task Get_Successful_ReturnResult(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabase.Get<object>(key);

            Assert.IsType<ValueTuple<object, string>>(result);
            Assert.Null(result.ToTuple().Item2);
        }

        [Theory]
        [SendKeyValidData]
        public async Task Get_FailedToConvertJson_ThrowsException(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabaseWithoutMultiplexer.Get<object>(key);

            Assert.IsType<ValueTuple<object, string>>(result);
            Assert.Null(result.ToTuple().Item1);
            Assert.NotNull(result.ToTuple().Item2);
        }
        #endregion

        #region IsCacheKeyValid
        [Theory]
        [SendKeyValidData]
        public async Task IsCacheKeyValid_Successfully_CallsIsValidMethod(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabase.IsCacheKeyValid(key);

            Assert.IsType<ValueTuple<bool, string>>(result);
        }
        #endregion

        #region Set
        [Theory]
        [SendWrongSyntaxedKeyInvalidData]
        public async Task Set_KeyIsNotValid_KeyIsNotValidResult(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabase.Set<object>(key, _redisExtensionFixture.JsonData);

            Assert.IsType<ValueTuple<bool, string>>(result);
            Assert.False(result.ToTuple().Item1);
        }

        #endregion

        #region RemoveKey
        [Theory]
        [SendKeyValidData]
        public async Task RemoveKey_Successfully_KeyRemoved(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabase.RemoveKey(key);

            Assert.IsType<ValueTuple<bool, string>>(result);
            Assert.True(result.ToTuple().Item1);
            Assert.Null(result.ToTuple().Item2);
        }

        [Theory]
        [SendWrongSyntaxedKeyInvalidData]
        public async Task RemoveKey_KeyIsNotValid_KeyIsNotValidResult(string key)
        {
            var result = await _redisExtensionFixture.RedisDatabase.RemoveKey(key);

            Assert.IsType<ValueTuple<bool, string>>(result);
            Assert.False(result.ToTuple().Item1);
        }
        #endregion
    }
}
