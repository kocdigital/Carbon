using Carbon.Common.TenantManagementHandler.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.WebApplication.TenantManagementHandler.Extensions
{
    public static class RoleExtensions
    {
        public static List<PermissionDetailedDto> PermissionDetailedDtos { get; set; }
        public static List<PermissionDetailedDto> SetPermissions(List<PermissionDetailedDto> permissionDetailedDtos)
        {
            PermissionDetailedDtos = permissionDetailedDtos;
            return PermissionDetailedDtos;
        }
        public static List<PermissionDetailedDto> GetPermissions()
        {
            return PermissionDetailedDtos;
        }
    }
}
