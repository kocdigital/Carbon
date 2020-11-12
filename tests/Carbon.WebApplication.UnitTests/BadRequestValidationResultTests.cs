using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Carbon.WebApplication.UnitTests
{
    public class TestService { }
    public class BadRequestValidationResultTests
    {
        private readonly Mock<ILogger<BadRequestValidationResult>> mockLogger;
        private readonly Mock<ILoggerFactory> mockLoggerFactory;
        private readonly BadRequestValidationResult _badRequestValidationResult;

        public BadRequestValidationResultTests()
        {
            mockLogger = new Mock<ILogger<BadRequestValidationResult>>();
            mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(c => c.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            mockLogger.Setup(c => c.IsEnabled(It.IsAny<LogLevel>())).Returns(false);
            _badRequestValidationResult = new BadRequestValidationResult(mockLogger.Object);
        }

        [Fact]
        public async Task ExecuteResultAsync_StateUnderTest_ExpectedBehavior()
        {
            var services = new ServiceCollection();
            services.AddMvcCore();
            services.AddOptions();
            services.AddSingleton<ILogger>(_ => mockLogger.Object);
            services.AddSingleton(_ => mockLoggerFactory.Object);
            services.AddScoped(_ => new TestService());
            var httpContextAccessorMock = Mock.Of<IHttpContextAccessor>();
            httpContextAccessorMock.HttpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider()
            };
            var httpContext = httpContextAccessorMock.HttpContext;
            var modelState = new ModelStateDictionary();
            httpContext.Request.Path = new PathString("/signin-activedirectory").ToUriComponent();

            var dic = new RouteValueDictionary("abd");
            var routeData = new RouteData(dic);
            ActionDescriptor ac = new ActionDescriptor();
            var actionContext = new ActionContext(
              httpContext,
              routeData,
              ac,
              modelState
          );
            var exception = await Record.ExceptionAsync(() => _badRequestValidationResult.ExecuteResultAsync(actionContext));
            Assert.Null(exception);
        }
    }
}