using Carbon.WebApplication.SolutionService.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.WebApplication.SolutionService.Services
{
    public interface ISolutionRegistrationService
    {
        Task AddAsSolution(SolutionCreationRequest data);

        Task AddAsFeatureSet(FeatureSetCreationRequest data);
    }
}
