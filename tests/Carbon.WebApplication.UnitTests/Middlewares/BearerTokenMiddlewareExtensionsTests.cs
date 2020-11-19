using Carbon.WebApplication.Middlewares;
using Microsoft.AspNetCore.Builder;
using Moq;
using System;
using Xunit;

namespace Carbon.WebApplication.UnitTests.Middlewares
{
    public class BearerTokenMiddlewareExtensionsTests
    {
        private readonly Mock<IServiceProvider> MockServiceProvider;

        public BearerTokenMiddlewareExtensionsTests()
        {
            MockServiceProvider = new Mock<IServiceProvider>();
        }

        [Fact]
        public void UseBearerTokenInRequestDto_ReturnApplicationBuilder()
        {
            // Arrange
            var appBuilder = new ApplicationBuilder(MockServiceProvider.Object);  

            // Act
            var res = appBuilder.UseBearerTokenInRequestDto();

            // Assert
            Assert.IsAssignableFrom<IApplicationBuilder>(res);
        }
    }
}
