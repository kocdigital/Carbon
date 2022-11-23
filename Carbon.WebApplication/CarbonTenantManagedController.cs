using Carbon.Common;
using Carbon.PagedList;
using Carbon.WebApplication.TenantManagementHandler.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Carbon.WebApplication
{
    /// <summary>
	/// <para>
	/// Adds automatic solution and ownership filtering. Uses OnActionExecuting event of <see cref="OwnershipFilter"/> and <see cref="SolutionFilter"/>, adds required filters to Query via repository on run time
	/// </para>
	/// </summary>
    public abstract class CarbonTenantManagedController : CarbonController, ISolutionFilteredController, IOwnershipFilteredController
    {

        public List<ISolutionFilteredService> SolutionFilteredServices { get; set; }
        public List<IOwnershipFilteredService> OwnershipFilteredServices { get; set; }

        public CarbonTenantManagedController(List<ISolutionFilteredService> solutionFilteredServices, List<IOwnershipFilteredService> ownershipFilteredServices) : base()
        {
            SolutionFilteredServices = solutionFilteredServices;
            OwnershipFilteredServices = ownershipFilteredServices;
        }

    }



}
