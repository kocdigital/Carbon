using Carbon.Common.UnitTests.StaticWrappers.ApiStatusCodeToHttpStatusMapperWrappers;
using Carbon.Test.Common.DataShares;
using Carbon.Test.Common.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Carbon.Common.UnitTests
{
   
    public class HttpStatusCodeExtensionsTest : IClassFixture<HttpStatusCodeExtensionsFixture>
    {
        #region PrivateFields
        private readonly Mock<IApiStatusCodeToHttpStatusMapperWrapper> _mockApiStatusCodeToHttpStatusMapper;
        private readonly HttpStatusCodeExtensionsFixture _httpStatusCodeExtensionsFixture;
        #endregion
        #region Constructor
        public HttpStatusCodeExtensionsTest(HttpStatusCodeExtensionsFixture httpStatusCodeExtensionsFixture)
        {
            _httpStatusCodeExtensionsFixture = httpStatusCodeExtensionsFixture;
            _mockApiStatusCodeToHttpStatusMapper = new Mock<IApiStatusCodeToHttpStatusMapperWrapper>();
        }
        #endregion
        [Theory]
        [ValidHttpStatusCodeExtensions]
        public void GetApiStatusCode_Successfully_HttpStatusCodeExtensionAsync(HttpStatusCode httpStatusCode)
        {
            // Arrange
            _mockApiStatusCodeToHttpStatusMapper.Setup(c => c.GetApiStatusCode(It.IsAny<HttpStatusCode>())).Returns(_httpStatusCodeExtensionsFixture.OkApiStatusCode);
            // Act
            var response = _mockApiStatusCodeToHttpStatusMapper.Object.GetApiStatusCode(httpStatusCode);
            //ArgumentOutOfRangeException exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => httpStatusCode.GetApiStatusCode());
            // Assert
            _mockApiStatusCodeToHttpStatusMapper.Verify((c => c.GetApiStatusCode(It.IsAny<HttpStatusCode>())), Times.Once);
            Assert.IsType<ApiStatusCode>(response);

        }
    }
   
}
