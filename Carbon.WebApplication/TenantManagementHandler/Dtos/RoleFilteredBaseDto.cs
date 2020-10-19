

using Carbon.Common;
using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.WebApplication.TenantManagementHandler.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Carbon.WebApplication
{
    /// <summary>
    /// A concrete class of any tenant filter managed Carbon request dto.
    /// </summary>
    internal class RoleFilteredConcreteDto : RoleFilteredBaseDto<RoleFilteredConcreteDto>
    {
        internal RoleFilteredConcreteDto()
        {

        }

        internal RoleFilteredConcreteDto(Guid ownerId, Guid organizationId, OwnerType ownerType)
        {
            this.OwnerId = ownerId;
            this.OrganizationId = organizationId;
            this.OwnerType = ownerType;
        }
    }

    /// <summary>
    /// Base class of any tenant filter managed Carbon request dto.
    /// </summary>
    public abstract class RoleFilteredBaseDto<T> : BaseRequestDto, IRoleFilteredDto
    {
        public RoleFilteredBaseDto()
        {

        }
        public RoleFilteredBaseDto(Guid ownerId, Guid organizationId, OwnerType ownerType)
        {
            this.OwnerId = ownerId;
            this.OrganizationId = organizationId;
            this.OwnerType = ownerType;
        }
        public Guid OwnerId { get; set; }
        public Guid OrganizationId { get; set; }
        public OwnerType OwnerType { get; set; }

        public bool ValidateFilter(List<PermissionDetailedDto> permissionDetailedDtos)
        {
            bool permitted = false;
            if (this.OwnerType == OwnerType.CustomerBased)
            {
                return true;
            }

            foreach (var permissionDetailedDto in permissionDetailedDtos)
            {
                if (permissionDetailedDto.PrivilegeLevelType == PermissionGroupImpactLevel.User)
                {
                    permitted |= (this.OwnerType == OwnerType.User || this.OwnerType == OwnerType.UserGroup) && this.OwnerId == permissionDetailedDto.UserId;
                }
                else if (permissionDetailedDto.PrivilegeLevelType == PermissionGroupImpactLevel.OnlyPolicyItself
                    || permissionDetailedDto.PrivilegeLevelType == PermissionGroupImpactLevel.PolicyItselfAndItsChildPolicies
                    || permissionDetailedDto.PrivilegeLevelType == PermissionGroupImpactLevel.AllPoliciesIncludedInZone)
                {
                    permitted |= (((this.OwnerType == OwnerType.User || this.OwnerType == OwnerType.Organization) && permissionDetailedDto.Policies.Contains(this.OrganizationId))
                        || (this.OwnerType == OwnerType.Role && this.OwnerId == permissionDetailedDto.RoleId));
                }
            }
            return permitted;
        }
    }
}
