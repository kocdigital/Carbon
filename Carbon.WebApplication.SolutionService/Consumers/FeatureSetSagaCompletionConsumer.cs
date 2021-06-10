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
    public class FeatureSetSagaCompletionSucceedConsumer : IConsumer<IFeatureSetCreationSucceed>
    {
        private ILogger<FeatureSetSagaCompletionSucceedConsumer> _logger;
        private readonly RequestedObjects _requestedObjects;

        public FeatureSetSagaCompletionSucceedConsumer(ILogger<FeatureSetSagaCompletionSucceedConsumer> logger, RequestedObjects requestedObjects)
        {
            _logger = logger;
            _requestedObjects = requestedObjects;

        }

        public async Task Consume(ConsumeContext<IFeatureSetCreationSucceed> context)
        {

            var message = context.Message;

            if (_requestedObjects.FeatureSets.Contains(message.CorrelationId))
            {
                _logger.LogInformation($"FeatureSet has successfully registered to SolutionId: {message.SolutionId}, FeatureSet: {message.FeatureSetId}");
            }
        }
    }

    public class FeatureSetSagaCompletionFailedConsumer : IConsumer<IFeatureSetCreationFailed>
    {
        private ILogger<FeatureSetSagaCompletionFailedConsumer> _logger;
        private readonly RequestedObjects _requestedObjects;

        public FeatureSetSagaCompletionFailedConsumer(ILogger<FeatureSetSagaCompletionFailedConsumer> logger, RequestedObjects requestedObjects)
        {
            _logger = logger;
            _requestedObjects = requestedObjects;

        }

        public async Task Consume(ConsumeContext<IFeatureSetCreationFailed> context)
        {
            var message = context.Message;
            if (_requestedObjects.FeatureSets.Contains(message.CorrelationId))
            {
                _logger.LogError($"FeatureSet registration has failed to SolutionId: {message.SolutionId}, FeatureSet: {message.FeatureSetId}, Error: {message.ErrorMessage}");
            }
        }
    }
}
