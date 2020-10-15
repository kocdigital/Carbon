using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.WebApplication.TenantManagementHandler.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.WebApplication.TenantManagementHandler.Services
{
    public interface IExternalService
    {
        public Task<List<PermissionDetailedDto>> ExecuteInPolicyApi_GetRoles(PermissionDetailedFilterDto permissionDetailedFilterDto, string token = null);
    }
}
