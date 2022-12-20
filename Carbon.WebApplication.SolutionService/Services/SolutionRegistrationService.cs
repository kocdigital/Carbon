using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Carbon.WebApplication.SolutionService.Domain;
using Carbon.WebApplication.SolutionService.Domain.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Platform360.Domain.Messages.SolutionCreationSaga;

namespace Carbon.WebApplication.SolutionService.Services
{
    public class SolutionRegistrationService : ISolutionRegistrationService
    {
        private ILogger<SolutionRegistrationService> _logger;
        private ISolutionRegistrationBus _busControl;

        public SolutionRegistrationService(ILogger<SolutionRegistrationService> logger,
                                  ISolutionRegistrationBus busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public async Task AddAsSolution(SolutionCreationRequest data)
        {
            if (data.Solution == null)
            {
                throw new SolutionNullException((int)ExceptionEnums.SolutionIdNull, "SolutionCreationRequest cannot be Null");
            }

            if (data.Solution.SolutionId == null)
            {
                throw new IncompatibleSolutionException((int)ExceptionEnums.SolutionIdNull, "SolutionCreationRequest SolutionId cannot be Null or Default");
            }

            if (data.Solution.SolutionName == null)
            {
                throw new IncompatibleSolutionException((int)ExceptionEnums.SolutionNameNull, "SolutionCreationRequest SolutionName cannot be Null or Default");
            }

            var sendEp = await _busControl.GetSendEndpoint(new Uri("exchange:solution-creation-state"));
            await sendEp.Send<ISolutionCreationRequest>(data);

            _logger.LogInformation($"Solution Registration has successfully submitted! SolutionId: {data.Solution.SolutionId} SolutionName: {data.Solution.SolutionName}");
        }

        public async Task AddAsFeatureSet(FeatureSetCreationRequest data)
        {
            if (data.SolutionId == Guid.Empty)
            {
                throw new SolutionNullException((int)ExceptionEnums.SolutionIdNull, "SolutionCreationRequest SolutionId cannot be Null or Default");
            }

            if (data.FeatureSet == null)
            {
                throw new FeatureSetNullException((int)ExceptionEnums.FeatureSetIdNull, "SolutionCreationRequest SolutionId cannot be Null or Default");
            }

            if (data.FeatureSet.FeatureSetId == null)
            {
                throw new IncompatibleSolutionException((int)ExceptionEnums.FeatureSetIdNull, "SolutionCreationRequest SolutionName cannot be Null or Default");
            }

            if (data.FeatureSet.FeatureSetName == null)
            {
                throw new IncompatibleSolutionException((int)ExceptionEnums.FeatureSetNameNull, "SolutionCreationRequest SolutionName cannot be Null or Default");
            }

            var sendEp = await _busControl.GetSendEndpoint(new Uri("exchange:featureset-creation-state"));
            await sendEp.Send<IFeatureSetCreationRequest>(data);

            _logger.LogInformation($"FeatureSet Registration has successfully submitted for given Solution {data.SolutionId} FeatureSet: {data.FeatureSet.FeatureSetName}");
        }


    }
}
