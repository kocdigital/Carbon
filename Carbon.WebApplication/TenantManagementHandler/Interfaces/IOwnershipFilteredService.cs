using Carbon.Common.TenantManagementHandler.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{
    /// <summary>
	/// Indicates that, implementing service is filterable by ownership
	/// </summary>
    public interface IOwnershipFilteredService
    {
        List<PermissionDetailedDto> FilterOwnershipList { get; set; }

        void SetFilter(List<PermissionDetailedDto> filters);
    }
}
