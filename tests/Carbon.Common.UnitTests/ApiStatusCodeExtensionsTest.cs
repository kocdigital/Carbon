using Carbon.Common.UnitTests.DataShares;
using Carbon.Common.UnitTests.StaticWrappers.ApiStatusCodeExtensionsWrapper;
using System.Net;
using Xunit;

namespace Carbon.Common.UnitTests
{
    public class ApiStatusCodeExtensionsTest
    {
        [Theory]
        [ValidApiStatusCode]
        public void GetHttpStatusCode_Successfully_ApiStatusCodeToApiStatusCodeExtensions(ApiStatusCode apiStatusCode)
        {
            // Arrange
            // Act
            var wrapper = new ApiStatusCodeExtensionsWrapper();
            var response = wrapper.GetHttpStatusCode(apiStatusCode);

            // Assert      
            Assert.IsType<HttpStatusCode>(response);

        }
    }
}
