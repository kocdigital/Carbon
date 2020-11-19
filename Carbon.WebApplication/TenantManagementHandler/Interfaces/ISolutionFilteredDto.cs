using Carbon.WebApplication.TenantManagementHandler.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{

    public interface ISolutionFilteredDto
    {
        ICollection<EntitySolutionRelationDto> RelationalOwners { get; set; }
    }

}
