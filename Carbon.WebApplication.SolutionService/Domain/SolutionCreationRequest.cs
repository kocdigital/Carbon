using System;
using System.Collections.Generic;

using Platform360.Domain.Messages.SolutionCreationSaga;
using Platform360.Domain.Messages.SolutionMenuItemEvents;

namespace Carbon.WebApplication.SolutionService.Domain
{
    public class SolutionCreationRequest : ISolutionCreationRequest
    {
        public Guid CorrelationId => Solution.SolutionId;
        
        public Solution Solution { get; set; }
        public List<SolutionMenuItemCreateDto> MenuItems { get; set; }
    }
}
