using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{
    public interface IOwnershipFilteredController
    {
        List<IOwnershipFilteredService> OwnershipFilteredServices { get; set; }
    }
}
