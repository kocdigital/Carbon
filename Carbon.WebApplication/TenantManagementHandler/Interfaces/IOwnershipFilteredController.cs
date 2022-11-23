using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{
    /// <summary>
	/// Indicates that, implementing controller contains operations that can be filterable by ownership via services that stated <see cref="OwnershipFilteredServices"/>
	/// </summary>
    public interface IOwnershipFilteredController
    {
        List<IOwnershipFilteredService> OwnershipFilteredServices { get; set; }
    }
}
