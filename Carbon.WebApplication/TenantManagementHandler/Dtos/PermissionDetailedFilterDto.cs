using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Dtos
{
    /// <summary>
	/// Request object for querying Policies from Policy API
	/// </summary>
    public class PermissionDetailedFilterDto
    {
        public Guid? UserId { get; set; }
        public Guid? SolutionId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? UserPolicyId { get; set; }
        public List<string> PermissionNames { get; set; }
    }
}
