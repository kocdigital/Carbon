using Carbon.Common;
using Carbon.PagedList;
using FluentValidation;
using Moq;
using Xunit;

namespace Carbon.WebApplication.UnitTests
{
    public class BaseDtoValidatorTests
    {
        [Fact]
        public void OrderableDto_Success()
        {
            // Act
            var x = Mock.Of<BaseDtoValidator<IOrderableDto>>();

            // Assert
            Assert.IsAssignableFrom<BaseDtoValidator<IOrderableDto>>(x);
        }

        [Fact]
        public void Valid_PageableDto()
        {
            // Act
            var x = Mock.Of<BaseDtoValidator<IPageableDto>>();

            // Assert
            Assert.IsAssignableFrom<BaseDtoValidator<IPageableDto>>(x);
        }
        [Fact]
        public void PageableDto_ValidPageNumberAndPageIndex()
        {
            // Act
            var x = Mock.Of<BaseDtoValidator<IPageableDto>>().ToPagedList(1, 250);

            // Assert
            Assert.IsAssignableFrom<PagedList<IValidationRule>>(x);
        }
    }
}