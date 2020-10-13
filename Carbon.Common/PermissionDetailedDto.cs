using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Common.TenantManagementHandler.Classes
{
    public class PermissionDetailedDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public PermissionType PermissionType { get; set; }
        public Guid PermissionGroupId { get; set; }
        public string PermissionGroupName { get; set; }
        public PermissionGroupImpactLevel PrivilegeLevelType { get; set; }
        public int? PrivilegeEffectLevel { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        /// <summary>
        /// If PrivilegeLevelType = 1, then it is blank
        /// If PrivilegeLevelType = 2, then it will be assigned with all the organizations selected on Role Provider
        /// If PrivilegeLevelType = 3, consider PrivilegeEffectLevel, call Tenant.API ChildOrganizationService for each organizations selected on Role Provider, and assign here distinctly
        /// If PrivilegeLevelType = 4, it is blank
        /// </summary>
        public List<Guid> Policies { get; set; }
    }

    public enum PermissionGroupImpactLevel
    {
        User = 1,
        OnlyPolicyItself = 2,
        PolicyItselfAndItsChildPolicies = 3,
        AllPoliciesIncludedInZone = 4
    }


    public enum PermissionType
    {
        EndpointItem = 1,
        MenuItem = 2,
        UIItem = 3
    }
}
