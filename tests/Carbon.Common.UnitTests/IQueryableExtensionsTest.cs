using Carbon.Common.UnitTests.DataShares;
using Carbon.Common.UnitTests.StaticWrappers.QueryableExtensionsWrapper;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.Common.UnitTests
{
    public class SampleTestClass
    {
        public string Name { get; set; }
    }
    public class StringEntity : IQueryableExtensionsTest<SampleTestClass> { public StringEntity(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public abstract class IQueryableExtensionsTest<TEntity>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        protected IQueryableExtensionsTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [CalculatePageCountQueryableExtensions]
        public void CalculatePageCount_Successfully_QueryableExtensions(IQueryable<TEntity> entity, int? size)
        {
            // Arrange
            // Act
            var wrapper = new QueryableExtensionsWrapper<TEntity>();
            var response = wrapper.CalculatePageCount(entity, size);

            // Assert      
            Assert.True((response == entity.Count()/(size?? entity.Count())));
        }

        [Theory]
        [SkipTakeQueryableExtensions]
        public void SkipTake_Successfully_QueryableExtensions(IQueryable<TEntity> entity, int index, int size)
        {
            // Arrange
            // Act
            var wrapper = new QueryableExtensionsWrapper<TEntity>();
            var response = wrapper.SkipTake(entity, index, size);

            // Assert      
            Assert.True(response.Any());
        }
        [Theory]
        [InvalidSkipTakeQueryableExtensions]
        public void SkipTake_SuccessfullyEmptyData_QueryableExtensions(IQueryable<TEntity> entity, int index, int size)
        {
            // Arrange
            // Act
            var wrapper = new QueryableExtensionsWrapper<TEntity>();
            var response = wrapper.SkipTake(entity, index, size);

            // Assert      
            Assert.Empty(response);
        }
        [Theory]
        [OrderByQueryableExtensions]
        public void OrderBy_Successfully_QueryableExtensions(IQueryable<TEntity> entity, ICollection<Orderable> orderables)
        {
            // Arrange
            // Act
            var wrapper = new QueryableExtensionsWrapper<TEntity>();
            var response = wrapper.OrderBy(entity, orderables);

            // Assert      
            Assert.NotNull(response);
        }
        [Theory]
        [EmptyDataOrderByQueryableExtensions]
        public void EmptyDataOrderBy_Successfully_QueryableExtensions(IQueryable<TEntity> entity, ICollection<Orderable> orderables)
        {
            // Arrange
            // Act
            var wrapper = new QueryableExtensionsWrapper<TEntity>();
            var response = wrapper.OrderBy(entity, orderables);

            // Assert      
            Assert.NotNull(response);
        }
    }
}
