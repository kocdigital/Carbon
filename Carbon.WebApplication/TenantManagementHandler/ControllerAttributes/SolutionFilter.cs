using Carbon.WebApplication.TenantManagementHandler.Extensions;
using Carbon.WebApplication.TenantManagementHandler.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.ControllerAttributes
{
    /// <summary>
	/// Adds required filters using OnActionExecuting event to Query via service's repository
	/// </summary>
    public class SolutionFilter : ActionFilterAttribute
    {
        public SolutionFilter()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var filterSolutionList = context.GetSolutionFilter();


            if (filterSolutionList != null && filterSolutionList.Any())
            {
                var relatedController = ((ISolutionFilteredController)context.Controller);
                relatedController.SolutionFilteredServices.ForEach(k => k.SetFilter(filterSolutionList));
            }
        }

    }
}
