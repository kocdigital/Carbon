using Carbon.MassTransit.UnitTests.DataShares;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Carbon.MassTransit.UnitTests
{
    public class MassTransitHostedServiceTest
    {
        private readonly Mock<IBusControl> _busControlMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly MassTransitHostedService _massTransitHostedService;
        public MassTransitHostedServiceTest()
        {
            _busControlMock = new Mock<IBusControl>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _massTransitHostedService = new MassTransitHostedService(_busControlMock.Object, _loggerFactoryMock.Object);
        }
        [Theory]
        [CancellationTokenData]
        public async Task StartAsync_Successfully_MassTransitHostedService(CancellationToken cancellationToken)
        {
            // Arrange
            // Act
            var exception = await Record.ExceptionAsync(() =>  _massTransitHostedService.StartAsync(cancellationToken));
            // Assert      
            Assert.Null(exception);
        }
        [Theory]
        [CancellationTokenData]
        public async Task StopAsync_Successfully_MassTransitHostedService(CancellationToken cancellationToken)
        {
            // Arrange
            // Act
            var exception = await Record.ExceptionAsync(() => _massTransitHostedService.StopAsync(cancellationToken));
            // Assert      
            Assert.Null(exception);
        }
    }
}
