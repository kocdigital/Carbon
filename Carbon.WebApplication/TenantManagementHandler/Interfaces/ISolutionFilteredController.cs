using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{
    public interface ISolutionFilteredController
    {
        List<ISolutionFilteredService> SolutionFilteredServices { get; set; }
    }
}
