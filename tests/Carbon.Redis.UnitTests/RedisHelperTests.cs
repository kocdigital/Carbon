using Carbon.Redis.UnitTests.DataShares;
using Carbon.Redis.UnitTests.Fixtures;
using Moq;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Carbon.Redis.UnitTests.StaticWrappers.RedisHelper;
using Xunit;


namespace Carbon.Redis.UnitTests
{
    public class RedisHelperTests : IClassFixture<RedisFixture>
    {
        #region Private Fields
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly RedisFixture _redisFixture;
        #endregion

        #region Constructor
        public RedisHelperTests(RedisFixture redisFixture)
        {
            _redisFixture = redisFixture;
            _mockDatabase = new Mock<IDatabase>();
        }
        #endregion

        #region ConvertObjectToJson
        [Theory]
        [ConvertObjectToJsonValidData]
        public async Task ConvertObjectToJson_Successfully_ConvertedResult(object obj)
        {
            var helper = new RedisHelperWrapper();
            var result = helper.ConvertObjectToJson(obj);

            Assert.IsType<string>(result);
            Assert.NotEmpty(result);
        }

        [Theory]
        [SendNullInvalidData]
        public async Task ConvertObjectToJson_FailedToConvert_EmptyResult(object obj)
        {
            var helper = new RedisHelperWrapper();
            var result = helper.ConvertObjectToJson(obj);

            Assert.IsType<string>(result);
            Assert.Empty(result);
        }
        #endregion

        #region ConvertJsonToObjectAsync
        [Theory]
        [SendKeyValidData]
        public async Task ConvertJsonToObject_Successfully_ConvertedResult(string key)
        {
            var helper = new RedisHelperWrapper();
            var result = await helper.ConvertJsonToObjectAsync<object>(_redisFixture.RedisDatabase, key);

            Assert.Null(result);
        }

        [Theory]
        [SendNullInvalidData]
        public async Task ConvertJsonToObject_FailedToConvert_EmptyResult(string key)
        {
            var helper = new RedisHelperWrapper();
            var result = await helper.ConvertJsonToObjectAsync<object>(_mockDatabase.Object, key);

            Assert.Null(result);
        }

        [Theory]
        [ConvertJsonToObjectExceptionalData]
        public async Task ConvertJsonToObject_FailedToConvert_ThrowsRedisException(IDatabase database, string key)
        {
            var helper = new RedisHelperWrapper();
            await Assert.ThrowsAsync<RedisException>(() => helper.ConvertJsonToObjectAsync<object>(database, key));
        }
        #endregion

        #region IsKeyValid
        [Theory]
        [SendKeyValidData]
        public async Task IsKeyValid_RedisDisabled_RedisDisabledResult(string key)
        {
            var helper = new RedisHelperWrapper();
            var result = await helper.IsKeyValid(key, _redisFixture.RedisDatabaseWithoutMultiplexer);
            
            Assert.IsType<ValueTuple<bool, string>>(result);
            Assert.NotNull(result.ToTuple().Item2);
            Assert.Equal(RedisConstants.RedisDisabled, result.ToTuple().Item2);
        }

        [Theory]
        [SendHugeKeyInvalidData]
        public async Task IsKeyValid_LengthError_ErrorResult(string key)
        {
            var helper = new RedisHelperWrapper();
            var result = await helper.IsKeyValid(key, _redisFixture.RedisDatabase);
            
            Assert.IsType<ValueTuple<bool, string>>(result);
            Assert.NotEmpty(result.ToTuple().Item2);
            Assert.False(result.ToTuple().Item1);
        }

        [Theory]
        [SendWrongSyntaxedKeyInvalidData]
        public async Task IsKeyValid_KeySyntaxError_ErrorResult(string key)
        {
            var helper = new RedisHelperWrapper();
            var result = await helper.IsKeyValid(key, _redisFixture.RedisDatabase);
            
            Assert.IsType<ValueTuple<bool, string>>(result);
            Assert.NotEmpty(result.ToTuple().Item2);
            Assert.False(result.ToTuple().Item1);
        }

        [Theory]
        [SendKeyValidData]
        public async Task IsKeyValid_Successful_ValidResult(string key)
        {
            var helper = new RedisHelperWrapper();
            var result = await helper.IsKeyValid(key, _redisFixture.RedisDatabase);
            
            Assert.IsType<ValueTuple<bool, string>>(result);
            Assert.Null(result.ToTuple().Item2);
            Assert.True(result.ToTuple().Item1);
        }
        #endregion

        #region IsRedisDisabled
        [Fact]
        public void IsRedisDisabled_Successfully_ReturnsFalse()
        {
            var helper = new RedisHelperWrapper();
            var result = helper.IsRedisDisabled(_redisFixture.RedisDatabase);

            Assert.False(result);
        }

        [Fact]
        public void IsRedisDisabled_RedisDisabled_ReturnsTrue()
        {
            var helper = new RedisHelperWrapper();
            var result = helper.IsRedisDisabled(_redisFixture.RedisDatabaseWithoutMultiplexer);

            Assert.True(result);
        } 
        #endregion
    }
}
