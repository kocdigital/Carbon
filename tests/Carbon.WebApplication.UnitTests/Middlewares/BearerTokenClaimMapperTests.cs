using Carbon.WebApplication.Middlewares;
using System.Collections.Generic;
using Xunit;

namespace Carbon.WebApplication.UnitTests.Middlewares
{
    public class BearerTokenClaimMapperTests
    {
        public BearerTokenClaimMapperTests()
        {

        }

        [Theory]
        [MemberData(nameof(GetValuePairs))]
        public void TryGetValue_MapSuccessfully(string key, string mappedKey, bool expectedResult)
        {
            // Arrange
            string outString;

            // Act
            var res = BearerTokenClaimMapper.TryGetValue(key, out outString);

            // Assert
            Assert.Equal(outString, mappedKey);
            Assert.Equal(res, expectedResult);
        }

        public static IEnumerable<object[]> GetValuePairs()
        {
            yield return new object[] { "sub", "ClientId", true };
            yield return new object[] { "tenant-id", "TenantId", true };
            yield return new object[] { "god-user", "GodUser", true };
            yield return new object[] { "random non existant key", null, false };
        }





    }
}
