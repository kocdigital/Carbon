using Platform360.Domain.Messages;
using Platform360.Domain.Messages.SolutionCreationSaga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carbon.WebApplication.SolutionService.Domain
{
    public class FeatureSetCreationRequest : IFeatureSetCreationRequest
    {
        public FeatureSetCreationRequest()
        {
            FeatureSet = new FeatureSet();
        }
        public FeatureSetCreationRequest(Guid solutionId, bool isDynamicSolution)
        {
            IsDynamicSolution = isDynamicSolution;
            SolutionId = solutionId;
            FeatureSet = new FeatureSet();
        }

        public Guid CorrelationId => FeatureSet.FeatureSetId;

        public FeatureSet FeatureSet { get; set; }
        public Guid SolutionId { get; set; }

        public bool IsDynamicSolution { get; set; }
    }
}
