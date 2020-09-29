using Carbon.MassTransit.Abstractions.UnitTests.DataShares;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.MassTransit.Abstractions.UnitTests
{
    public class DecimalFacts : MassTransitMessageTest<decimal, string> { public DecimalFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public abstract class MassTransitMessageTest<TType, TContent>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        protected MassTransitMessageTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        [Theory]
        [MassTransitMessageContentConstractor]
        public void CreateMassTransitMessage_Successfully_MassTransit(TContent content)
        {
            // Arrange
            // Act
            MassTransitMessage<TType, TContent> response = new MassTransitMessage<TType, TContent>(content);
            // Assert

            Assert.Equal(content, response.Content);
            Assert.NotNull(response.MessageType);

            _testOutputHelper.WriteLine("Test passed!");
        }

        [Fact]
        public void CreateMassTransitMessageWithEmptyConstractor_Successfully_MassTransit()
        {
            // Arrange
            // Act
            MassTransitMessage<TType, TContent> response = new MassTransitMessage<TType, TContent>();
            // Assert

            Assert.Null(response.Content);
            Assert.Null(response.MessageType);

            _testOutputHelper.WriteLine("Test passed!");
        }
    }
}
