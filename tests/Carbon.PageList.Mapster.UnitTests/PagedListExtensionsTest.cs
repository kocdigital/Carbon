using System;
using Xunit;
using Carbon.Test.Common.DataShares;
using Xunit.Abstractions;
using System.Linq;
using Carbon.PagedList;
using Carbon.PageList.Mapster.UnitTests.DataShares;
using Carbon.PageList.Mapster.UnitTests.StaticWrappers;

namespace Carbon.PageList.Mapster.UnitTests
{
    public class StringEntity : PagedListExtensionsTest<CarbonContextTestClass,CarbonContextTestDummyClass> { public StringEntity(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public abstract class PagedListExtensionsTest<TEntity,TOutputEntity>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        protected PagedListExtensionsTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [AdaptQueryableExtensions]
        public void Adapt_Successfully_PagedListExtensions(IPagedList<TEntity> entity )
        {
            // Arrange
            // Act
            var wrapper = new QueryableExtensionsWrapper<TEntity, TOutputEntity>();
            var response = wrapper.Adapt(entity);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<StaticPagedList<TOutputEntity>>(response);

            _testOutputHelper.WriteLine("Test passed!");
        }

        [Theory]
        [InValidAdaptQueryableExtensions]
        public void Adapt_Exception_PagedListExtensions(IPagedList<TEntity> entity)
        {
            // Arrange
            // Act
            var wrapper = new QueryableExtensionsWrapper<TEntity, TOutputEntity>();

            // Assert
            Assert.Throws<ArgumentNullException>(() =>  wrapper.Adapt(entity));

            _testOutputHelper.WriteLine("Test passed!");
        }
    }
}
