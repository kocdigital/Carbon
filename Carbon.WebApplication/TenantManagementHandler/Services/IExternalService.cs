using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.WebApplication.TenantManagementHandler.Dtos;
using Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.WebApplication.TenantManagementHandler.Services
{
    /// <summary>
	/// Contains methods for generic external APIs like Policy and Error
	/// </summary>
    public interface IExternalService
    {
        /// <summary>
		/// Gets policies from Policy API
		/// </summary>
		/// <param name="permissionDetailedFilterDto">Request object for querying Policies from Policy API</param>
		/// <param name="token"></param>
		/// <returns>List of <see cref="PermissionDetailedDto"/></returns>
        public Task<List<PermissionDetailedDto>> ExecuteInPolicyApi_GetRoles(PermissionDetailedFilterDto permissionDetailedFilterDto, string token = null);

        /// <summary>
        /// Gets application based translated errors
        /// </summary>
        /// <param name="request">Request object for to get application based errors for given language and tenant</param>
        /// <param name="token"></param>
        /// <returns><see cref="ErrorResponse"/></returns>
        public Task<bool> RegisterApplicationError(ApplicationErrorRegisterRequest request, string token = null);
        
        /// <summary>
		/// Adds application based translated errors
		/// </summary>
		/// <param name="request">Request object for creating application based translated error definitions</param>
		/// <param name="token"></param>
		/// <returns><c>True</c> if request is succesfull</returns>
		public Task<ErrorResponse> GetErrorDescription(ApplicationErrorRequest request, string token = null);
    }
}
