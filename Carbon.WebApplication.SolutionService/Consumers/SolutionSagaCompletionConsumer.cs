using Carbon.WebApplication.SolutionService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Platform360.Domain.Messages;
using Platform360.Domain.Messages.SolutionCreationSaga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carbon.WebApplication.SolutionService.Consumers
{
    public class SolutionSagaCompletionSucceedConsumer : IConsumer<ISolutionCreationCompleted>
    {
        private ILogger<SolutionSagaCompletionSucceedConsumer> _logger;
        private readonly RequestedObjects _requestedObjects;
        public SolutionSagaCompletionSucceedConsumer(ILogger<SolutionSagaCompletionSucceedConsumer> logger, RequestedObjects requestedObjects)
        {
            _logger = logger;
            _requestedObjects = requestedObjects;
        }

        public async Task Consume(ConsumeContext<ISolutionCreationCompleted> context)
        {
            var message = context.Message;

            if (_requestedObjects.Solutions.Contains(message.CorrelationId))
            {
                _logger.LogInformation($"Solution has successfully registered- SolutionId: {message.SolutionId}, SolutionName: {message.SolutionName}");
            }
        }
    }

    public class SolutionSagaCompletionFailedConsumer : IConsumer<ISolutionCreationFailed>
    {
        private ILogger<SolutionSagaCompletionFailedConsumer> _logger;
        private readonly RequestedObjects _requestedObjects;

        public SolutionSagaCompletionFailedConsumer(ILogger<SolutionSagaCompletionFailedConsumer> logger, RequestedObjects requestedObjects)
        {
            _logger = logger;
            _requestedObjects = requestedObjects;

        }

        public async Task Consume(ConsumeContext<ISolutionCreationFailed> context)
        {
            var message = context.Message;
            if (_requestedObjects.Solutions.Contains(message.CorrelationId))
            {
                _logger.LogError($"Solution registration has failed- SolutionId: {message.SolutionId}, SolutionName: {message.SolutionName}, Error: {message.ErrorMessage}");
            }
        }
    }
}
