using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Common.TenantManagementHandler.Interfaces
{
    public interface ISolutionFilteredRepository
    {
        List<Guid> FilterSolutionList { get; set; }

        void SetSolutionFilter(List<Guid> filters);
    }
}
