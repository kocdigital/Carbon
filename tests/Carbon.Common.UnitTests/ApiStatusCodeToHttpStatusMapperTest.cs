using Carbon.Common;
using Carbon.Common.UnitTests.StaticWrappers.ApiStatusCodeToHttpStatusMapperWrappers;
using Carbon.Common.UnitTests.StaticWrappers.HttpStatusCodesWrapper;
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
    public class ApiStatusCodeToHttpStatusMapperTest : IClassFixture<ApiStatusCodeToHttpStatusMapperFixture>
    {
        #region PrivateFields
        private readonly Mock<IHttpStatusCodesWrapper<HttpStatusCode, ApiStatusCode>> _mockHttpStatusCodesWrapper;
        private readonly ApiStatusCodeToHttpStatusMapperFixture _apiStatusCodeToHttpStatusMapperFixture;
        #endregion
        #region Constructor
        public ApiStatusCodeToHttpStatusMapperTest(ApiStatusCodeToHttpStatusMapperFixture apiStatusCodeToHttpStatusMapperFixture)
        {
            _apiStatusCodeToHttpStatusMapperFixture = apiStatusCodeToHttpStatusMapperFixture;
            _mockHttpStatusCodesWrapper = new Mock<IHttpStatusCodesWrapper<HttpStatusCode, ApiStatusCode>>();
        }
        #endregion
        //[Theory]
        //[ValidHttpStatusCodeExtensions]
        //public async Task GetApiStatusCode_Successfully_ApiStatusCodeToHttpStatusMapper(HttpStatusCode httpStatusCode, ApiStatusCode apiStatusCode)
        //{
        //    // Arrange
        //    _mockHttpStatusCodesWrapper.Setup(c => c.TryGetValue(It.IsAny<HttpStatusCode>(),  It.IsAny<ApiStatusCode>())).Returns(_apiStatusCodeToHttpStatusMapperFixture.Ok);
        //    // Act
        //    var response = ApiStatusCodeToHttpStatusMapper.GetApiStatusCode(httpStatusCode);
        //    //ArgumentOutOfRangeException exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => httpStatusCode.GetApiStatusCode());
        //    // Assert
        //    _mockHttpStatusCodesWrapper.Verify((c => c.TryGetValue(It.IsAny<HttpStatusCode>())), Times.Once);
        //    Assert.IsType<ApiStatusCode>(response);

        //}
    }
}
