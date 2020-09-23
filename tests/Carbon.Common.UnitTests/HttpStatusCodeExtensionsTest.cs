using Carbon.Common.UnitTests.StaticWrappers.HttpStatusCodeExtensionsWrapper;
using Carbon.Test.Common.DataShares;
using System.Net;
using Xunit;

namespace Carbon.Common.UnitTests
{

    public class HttpStatusCodeExtensionsTest
    {


        [Theory]
        [ValidApiStatusCodeToApiStatusCodeToHttpStatusMapper]
        public void GetApiStatusCode_Successfully_ApiStatusCodeToHttpStatusMapper(HttpStatusCode httpStatusCode)
        {
            // Arrange
            // Act
            var httpStatus = new HttpStatusCodeExtensionsWrapper();
            var response = httpStatus.GetApiStatusCode(httpStatusCode);

            // Assert      
            Assert.IsType<ApiStatusCode>(response);

        }
    }

}
