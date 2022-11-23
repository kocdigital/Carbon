using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Interfaces
{
    /// <summary>
	/// Indicates that, implementing service is filterable by solution
	/// </summary>
    public interface ISolutionFilteredService
    {
        List<Guid> FilterSolutionList { get; set; }

        void SetFilter(List<Guid> filters);
    }
}
