using Carbon.WebApplication.UnitTests.DataShares;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Carbon.WebApplication.UnitTests
{
    public class InternalServerErrorObjectResultTest
    {
        [Theory]
        [InternalServerErrorObjectResultData]
        public void CreateWithInternalServerError_CreateSuccessfully_ReturnObjectResult(object error)
        {
            // Act
            var response = new InternalServerErrorObjectResult(error);

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
            Assert.Equal(error, response.Value);
            Assert.IsType<InternalServerErrorObjectResult>(response);
        }
    }
}
