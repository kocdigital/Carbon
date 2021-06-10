using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.SolutionService.Domain
{
    public class RequestedObjects
    {
        public RequestedObjects()
        {

        }
        public List<Guid> FeatureSets { get; set; }
        public List<Guid> Solutions { get; set; }

    }
}
