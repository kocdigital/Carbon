using Carbon.Common;
using Carbon.Common.TenantManagementHandler.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{
    public interface IRoleFilteredDto
    {
        Guid OwnerId { get; set; }
        Guid OrganizationId { get; set; }
        OwnerType OwnerType { get; set; }

        bool ValidateFilter(List<PermissionDetailedDto> permissionDetailedDtos);
    }
}
