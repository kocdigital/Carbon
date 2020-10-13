using Carbon.Common.TenantManagementHandler.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Common.TenantManagementHandler.Interfaces
{
    public interface IOwnershipFilteredRepository
    {
        List<PermissionDetailedDto> FilterOwnershipList { get; set; }

        void SetOwnershipFilter(List<PermissionDetailedDto> filters);
    }
}
