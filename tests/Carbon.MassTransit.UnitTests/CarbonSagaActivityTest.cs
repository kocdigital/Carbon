using Carbon.MassTransit.UnitTests.DataShares;
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

    public class TestSagaInstance : CarbonStateMachineInstance
    { }
    public class TestSagaMessage
    { }
    public class DigitFacts : CarbonSagaActivityTest<TestSagaInstance, TestSagaMessage>
    { }
    public abstract class CarbonSagaActivityTest<T, T1>
        where T : class, SagaStateMachineInstance
        where T1 : class
    {
        private readonly Mock<BehaviorExceptionContext<T, T1, Exception>> _behaviorExceptionContextFaultedMock;
        private readonly Mock<BehaviorExceptionContext<T, Exception>> _behaviorExceptionContextMock;
        private readonly Mock<IBehavior<T, T1>> _behaviorFaultedMock;
        private readonly Mock<IBehavior<T>> _behaviorMock;
        private readonly Mock<BehaviorContext<T, T1>> _behaviorContextExecuteMock;
        private readonly Mock<BehaviorContext<T>> _behaviorContextMock;
        private readonly Mock<StateMachineVisitor> _stateMachineVisitorMock;
        private readonly Mock<ILogger<T>> _loggerMock;
        private readonly Mock<ProbeContext> _probeContextMock;
        private readonly Mock<ConsumeContext> _consumeContextMock;
        private readonly Mock<CarbonSagaActivity<T>> _carbonSagaActivityMock;

        public CarbonSagaActivityTest()
        {
            _behaviorFaultedMock = new Mock<IBehavior<T, T1>>();
            _behaviorExceptionContextFaultedMock = new Mock<BehaviorExceptionContext<T, T1, Exception>>();
            _behaviorExceptionContextMock = new Mock<BehaviorExceptionContext<T, Exception>>();
            _loggerMock = new Mock<ILogger<T>>();
            _consumeContextMock = new Mock<ConsumeContext>();
            _carbonSagaActivityMock = new Mock<CarbonSagaActivity<T>>(_loggerMock.Object, _consumeContextMock.Object);
            _stateMachineVisitorMock = new Mock<StateMachineVisitor>();
            _probeContextMock = new Mock<ProbeContext>();
            _behaviorContextMock = new Mock<BehaviorContext<T>>();
            _behaviorMock = new Mock<IBehavior<T>>();
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
        public async Task ExecuteT1_ForwardsTo_Execute_CarbonSagaActivity()
        {
            // Arrange
            _carbonSagaActivityMock
                .Setup(a => a.Execute(It.IsAny<BehaviorContext<T>>(), It.IsAny<IBehavior<T>>()))
                .Returns<BehaviorContext<T>, IBehavior<T>>((context, next) => next.Execute(context));
            _behaviorFaultedMock
                .Setup(b => b.Execute(It.IsAny<BehaviorContext<T, T1>>()))
                .Returns(Task.CompletedTask);
            // Act
            await _carbonSagaActivityMock.Object.Execute(_behaviorContextExecuteMock.Object, _behaviorFaultedMock.Object);
            // Assert
            _carbonSagaActivityMock.Verify(a => a.Execute(_behaviorContextExecuteMock.Object, It.IsAny<IBehavior<T>>()), Times.Once);
            _behaviorFaultedMock.Verify(b => b.Execute(_behaviorContextExecuteMock.Object), Times.Once);
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
