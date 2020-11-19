using Carbon.WebApplication.UnitTests.DataShares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Xunit;

namespace Carbon.WebApplication.UnitTests
{
    public class ValidateModelFilterUnitTests
    {
        private readonly MockRepository mockRepository;

        public ValidateModelFilterUnitTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
        }

        private ValidateModelFilter CreateValidateModelFilter()
        {
            return new ValidateModelFilter();
        }

        [Fact]
        public void OnActionExecuted_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var validateModelFilter = this.CreateValidateModelFilter();
            ActionExecutedContext context = null;

            // Act
            validateModelFilter.OnActionExecuted(context);

            // Assert
            Assert.True(true);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [Data]
        public void OnActionExecuting_StateUnderTest_ExpectedBehavior(ActionExecutingContext actionExecutingContext)
        {
            // Arrange
            var validationFilter = new ValidateModelFilter();

            // Act
            validationFilter.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
        }
    }
}