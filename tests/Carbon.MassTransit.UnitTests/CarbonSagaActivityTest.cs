using Automatonymous;
using Carbon.MassTransit.UnitTests.DataShares;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Carbon.MassTransit.UnitTests
{

    public class DigitFacts : CarbonSagaActivityTest<decimal, decimal>
    { }
    public abstract class CarbonSagaActivityTest<T, T1>
    {
        private readonly Mock<BehaviorExceptionContext<T, T1, Exception>> _behaviorExceptionContextFaultedMock;
        private readonly Mock<BehaviorExceptionContext<T, Exception>> _behaviorExceptionContextMock;
        private readonly Mock<Behavior<T, T1>> _behaviorFaultedMock;
        private readonly Mock<Behavior<T>> _behaviorMock;
        private readonly Mock<BehaviorContext<T, T1>> _behaviorContextExecuteMock;
        private readonly Mock<BehaviorContext<T>> _behaviorContextMock;
        private readonly Mock<StateMachineVisitor> _stateMachineVisitorMock;
        private readonly Mock<ILogger<T>> _loggerMock;
        private readonly Mock<ProbeContext> _probeContextMock;
        private readonly Mock<ConsumeContext> _consumeContextMock;
        private readonly Mock<CarbonSagaActivity<T>> _carbonSagaActivityMock;

        public CarbonSagaActivityTest()
        {
            _behaviorFaultedMock = new Mock<Behavior<T, T1>>();
            _behaviorExceptionContextFaultedMock = new Mock<BehaviorExceptionContext<T, T1, Exception>>();
            _behaviorExceptionContextMock = new Mock<BehaviorExceptionContext<T, Exception>>();
            _loggerMock = new Mock<ILogger<T>>();
            _consumeContextMock = new Mock<ConsumeContext>();
            _carbonSagaActivityMock = new Mock<CarbonSagaActivity<T>>(_loggerMock.Object, _consumeContextMock.Object);
            _stateMachineVisitorMock = new Mock<StateMachineVisitor>();
            _probeContextMock = new Mock<ProbeContext>();
            _behaviorContextMock = new Mock<BehaviorContext<T>>();
            _behaviorMock = new Mock<Behavior<T>>();
            _behaviorContextExecuteMock = new Mock<BehaviorContext<T, T1>>();
        }
        [Fact]
        public void Accept_Successfully_CarbonSagaActivity()
        {
            // Arrange
            // Act
            var exception =  Record.Exception(() => _carbonSagaActivityMock.Object.Accept(_stateMachineVisitorMock.Object));
            // Assert      
            Assert.Null(exception);
        }

     

        [Fact]
        public void Probe_Successfully_CarbonSagaActivity()
        {
            // Arrange
            // Act
            var exception = Record.Exception(() => _carbonSagaActivityMock.Object.Probe(_probeContextMock.Object));
            // Assert      
            Assert.Null(exception);
        }

        [Fact]
        public async Task Execute_Successfully_CarbonSagaActivity()
        {
            // Arrange
            // Act
            var exception = await Record.ExceptionAsync(() => _carbonSagaActivityMock.Object.Execute(_behaviorContextMock.Object, _behaviorMock.Object));
            // Assert      
            Assert.Null(exception);
        }
        [Fact]
        public async Task ExecuteT1_Successfully_CarbonSagaActivity()
        {
            // Arrange
            // Act
            var exception = await Record.ExceptionAsync(() => _carbonSagaActivityMock.Object.Execute(_behaviorContextExecuteMock.Object, _behaviorFaultedMock.Object));
            // Assert      
            Assert.Null(exception);
        }
        [Fact]
        public async Task Faulted_Successfully_CarbonSagaActivity()
        {
            // Arrange
            // Act
            var exception = await Record.ExceptionAsync(() => _carbonSagaActivityMock.Object.Faulted(_behaviorExceptionContextMock.Object, _behaviorMock.Object));
            // Assert      
            Assert.Null(exception);
        }

        [Fact]
        public async Task FaultedT1_Successfully_CarbonSagaActivity()
        {
            // Arrange
            // Act
            var exception = await Record.ExceptionAsync(() => _carbonSagaActivityMock.Object.Faulted(_behaviorExceptionContextFaultedMock.Object, _behaviorFaultedMock.Object));
            // Assert      
            Assert.Null(exception);
        }
    }
}
