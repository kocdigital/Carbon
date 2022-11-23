using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{
    /// <summary>
	/// Indicates that, implementing controller contains operations that can be filterable by solution  via services that stated <see cref="SolutionFilteredServices"/>
	/// </summary>
    public interface ISolutionFilteredController
    {
        List<ISolutionFilteredService> SolutionFilteredServices { get; set; }
    }
}
