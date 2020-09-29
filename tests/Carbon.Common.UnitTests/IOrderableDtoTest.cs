using Carbon.Common.UnitTests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.Common.UnitTests
{
    public class IOrderableDtoTest : IClassFixture<OrderableFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly OrderableFixture _orderableFixture;
        public IOrderableDtoTest(ITestOutputHelper testOutputHelper, OrderableFixture orderableFixture)
        {
            _testOutputHelper = testOutputHelper;
            _orderableFixture = orderableFixture;
        }
        [Fact]
        public void SetValuesToOrdarableDto_Successfully_OrdarableDto()
        {
            // Arrange
            // Act
            var ordarable = _orderableFixture.Orderable;
            // Assert      
            Assert.NotNull(ordarable.Value);
            Assert.True(ordarable.IsAscending);

            _testOutputHelper.WriteLine("Test passed!");
        }
    }
}
