using Carbon.Common.UnitTests.StaticWrappers.ApiStatusCodeToHttpStatusMapperWrappers;
using Carbon.Test.Common.DataShares;
using System;
using System.Net;
using Xunit;

namespace Carbon.Common.UnitTests
{
    public class ApiStatusCodeToHttpStatusMapperTest
    {


        [Theory]
        [ValidApiStatusCodeToApiStatusCodeToHttpStatusMapper]
        public void GetApiStatusCode_Successfully_ApiStatusCodeToHttpStatusMapper(HttpStatusCode httpStatusCode)
        {
            // Arrange
            // Act
            var apiStatus = new ApiStatusCodeToHttpStatusMapperWrapper();
            var response = apiStatus.GetApiStatusCode(httpStatusCode);

            // Assert      
            Assert.IsType<ApiStatusCode>(response);

        }

        [Theory]
        [InValidApiStatusCodeToApiStatusCodeToHttpStatusMapper]
        public void GetApiStatusCode_Exception_ApiStatusCodeToHttpStatusMapper(HttpStatusCode httpStatusCode)
        {
            // Arrange 
            // Act
            var apiStatus = new ApiStatusCodeToHttpStatusMapperWrapper();
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => apiStatus.GetApiStatusCode(httpStatusCode));
            // Assert


        }


        [Theory]
        [ValidHttpStatusCodeToApiStatusCodeToHttpStatusMapper]
        public void GetHttpStatusCode_Successfully_ApiStatusCodeToHttpStatusMapper(ApiStatusCode apiStatusCode)
        {
            // Arrange
            // Act
            var apiStatus = new ApiStatusCodeToHttpStatusMapperWrapper();
            var response = apiStatus.GetHttpStatusCode(apiStatusCode);

            // Assert      
            Assert.IsType<HttpStatusCode>(response);

        }

        [Theory]
        [InValidHttpStatusCodeToApiStatusCodeToHttpStatusMapper]
        public void GetHttpStatusCode_Exception_ApiStatusCodeToHttpStatusMapper(ApiStatusCode apiStatusCode)
        {
            // Arrange 
            // Act
            var apiStatus = new ApiStatusCodeToHttpStatusMapperWrapper();
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => apiStatus.GetHttpStatusCode(apiStatusCode));
            // Assert


        }
    }
}
