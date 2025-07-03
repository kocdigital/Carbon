using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.ExceptionHandling.Abstractions;
using Carbon.WebApplication.TenantManagementHandler.Dtos;
using Carbon.WebApplication.TenantManagementHandler.Extensions;
using Carbon.WebApplication.TenantManagementHandler.Interfaces;
using Carbon.WebApplication.TenantManagementHandler.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carbon.WebApplication.TenantManagementHandler.ControllerAttributes
{
    public enum OwnershipType
    {
        User = 0,
        Admin = 1,
        Superadmin = 2,
        Goduser = 99
    }

    /// <summary>
    /// Adds required filters using OnActionExecuting event to Query via service's repository
    /// </summary>
    /// <remarks>
    /// Checks Superadmin and Admin rights. If user is GodUser skips adding filter to query
    /// </remarks>
    public class OwnershipFilter : ActionFilterAttribute
    {
        private readonly string _role;
        private readonly OwnershipType _ownershipType;

        public OwnershipFilter(string role = null)
        {
            _role = role;
            _ownershipType = OwnershipType.User;
        }

        public OwnershipFilter(OwnershipType ownershipType = OwnershipType.User, string role = null)
        {
            _role = role;
            _ownershipType = ownershipType;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
	    var httpContext = context.HttpContext;
	    var controller = (CarbonController)context.Controller;
	    var user = controller.User;
	
	    var isGodUser = user.CheckIfGodUser();
	
	    if (_ownershipType == OwnershipType.Goduser && !isGodUser)
	        throw new ForbiddenOperationException(CarbonExceptionMessages.OnlyGodUserOperation);
	
	    if (isGodUser)
	    {
	        await next();
	        return;
	    }
	
	    var _externalService = httpContext.RequestServices.GetService<IExternalService>();
	    var userId = user.GetUserId();
	    var tenantId = user.GetTenantId();
	    var organizationId = user.GetOrganizationId();
	
	    httpContext.Request.Headers.TryGetValue("Authorization", out StringValues authHeader);
	    var tokenParts = authHeader.ToString().Split(' ');
	    var token = tokenParts.Length > 1 ? tokenParts[1] : null;
	
	    if (string.IsNullOrEmpty(token))
	    {
	        httpContext.Response.StatusCode = 401;
	        context.Result = new JsonResult(new { response = "Unauthorized", message = "Authorization header is invalid or missing token." });
	        return;
	    }
	
	    List<PermissionDetailedDto> filterOwnershipPermissionList;
	
	    filterOwnershipPermissionList = await _externalService.ExecuteInPolicyApi_GetRoles(
	        new PermissionDetailedFilterDto()
	        {
	            TenantId = tenantId,
	            UserPolicyId = organizationId,
	            UserId = userId,
	            SolutionId = null,
	            PermissionNames = new List<string>() { _role }
	        }, token);
	
	    RoleExtensions.SetPermissions(filterOwnershipPermissionList);
	
	    if (_ownershipType == OwnershipType.Admin)
	    {
	        if (filterOwnershipPermissionList == null || !filterOwnershipPermissionList.Any(k => k.OriginatedRoleType <= 2))
	            throw new ForbiddenOperationException(CarbonExceptionMessages.OnlyAdminUserOperation);
	    }
	
	    if (_ownershipType == OwnershipType.Superadmin)
	    {
	        if (filterOwnershipPermissionList == null || !filterOwnershipPermissionList.Any(k => k.OriginatedRoleType == 1))
	            throw new ForbiddenOperationException(CarbonExceptionMessages.OnlySuperAdminUserOperation);
	    }
	
	    if (filterOwnershipPermissionList != null && filterOwnershipPermissionList.Any())
	    {
	        var relatedController = (IOwnershipFilteredController)context.Controller;
	        relatedController.OwnershipFilteredServices.ForEach(k => k.SetFilter(filterOwnershipPermissionList));
	        await next();
	    }
	    else
	    {
	        httpContext.Response.StatusCode = 403;
	        context.Result = new JsonResult(new { response = "No_Permission", message = _role + " permission not found!" });
	    }
	}
    }
}
