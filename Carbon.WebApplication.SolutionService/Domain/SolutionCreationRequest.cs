using System;
using System.Collections.Generic;
using Platform360.Domain.Messages;
using Platform360.Domain.Messages.SolutionCreationSaga;

namespace Carbon.WebApplication.SolutionService.Domain
{
    public class SolutionCreationRequest : ISolutionCreationRequest
    {
        public SolutionCreationRequest()
        {

        }

        public Guid CorrelationId { 
            get 
            { 
                return this.Solution.SolutionId; 
            } 
        }
        public Solution Solution { get; set; }
    }
}
