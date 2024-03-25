using Carbon.PagedList.EntityFrameworkCore.UnitTests.DataShares;
using Carbon.PagedList.EntityFrameworkCore.UnitTests.StaticWrappers.QueryableExtensionsWrapper;
using Carbon.Test.Common.DataShares;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.PagedList.EntityFrameworkCore.UnitTests
{
    public class StringEntity : PagedListExtensionsTest<CarbonContextTestClass> { public StringEntity(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public abstract class PagedListExtensionsTest<TEntity>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        protected PagedListExtensionsTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ToPagedListQueryableExtensions]
        public void ToPagedListAsync_Successfully_PagedListExtensions(IQueryable<TEntity> entity, int pageNumber, int pageSize)
        {
            // Arrange
            // Act
            var wrapper = new QueryableExtensionsWrapper<TEntity>();
            var response = wrapper.ToPagedListAsync(entity, pageNumber, pageSize);

            // Assert
            Assert.Null(response.Exception);
            Assert.IsType<PagedList<TEntity>>(response.Result);

            _testOutputHelper.WriteLine("Test passed!");
        }
    }
}