using Carbon.Common;
using Carbon.WebApplication.TenantManagementHandler.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{

    public interface IOwnershipFilteredDto
    {
        Guid OwnerId { get; set; }
        Guid OrganizationId { get; set; }
        OwnerType OwnerType { get; set; }
    }

}
