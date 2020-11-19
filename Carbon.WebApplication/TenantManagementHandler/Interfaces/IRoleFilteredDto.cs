using Carbon.Common;
using Carbon.Common.TenantManagementHandler.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{
    public interface IRoleFilteredDto : IOwnershipFilteredDto
    {
        bool ValidateFilter(List<PermissionDetailedDto> permissionDetailedDtos);
    }
}
