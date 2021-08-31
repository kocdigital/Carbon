using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.WebApplication.TenantManagementHandler.Dtos;
using Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.WebApplication.TenantManagementHandler.Services
{
    public interface IExternalService
    {
        public Task<List<PermissionDetailedDto>> ExecuteInPolicyApi_GetRoles(PermissionDetailedFilterDto permissionDetailedFilterDto, string token = null);
        public Task<bool> RegisterApplicationError(ApplicationErrorRegisterRequest request, string token = null);
        public Task<ErrorResponse> GetErrorDescription(ApplicationErrorRequest request, string token = null);
    }
}
