using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Extensions
{
    /// <summary>
    /// Provides some useful token parse operations
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Retrieve User Id Claim from Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid? GetUserId(this System.Security.Claims.ClaimsPrincipal user)
        {
            var guidUsr = user.Claims?.Where(k => k.Type.Contains("claims/nameidentifier"))?.FirstOrDefault()?.Value;

            if (String.IsNullOrEmpty(guidUsr))
                return null;
            else
                return new Guid(guidUsr);
        }
        /// <summary>
        /// Retrieve User Name Claim from Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetUserFullName(this System.Security.Claims.ClaimsPrincipal user)
        {
            return user.Claims?.Where(k => k.Type.Contains("fullname"))?.FirstOrDefault()?.Value;
        }


        /// <summary>
        /// Retrieve User Name Claim from Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetUserName(this System.Security.Claims.ClaimsPrincipal user)
        {
            return user.Claims?.FirstOrDefault(k => k.Type == "name")?.Value ?? GetUserFullName(user) ?? user.Identity?.Name;
        }

        /// <summary>
        /// Retrieve TenantId claim from Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid? GetTenantId(this System.Security.Claims.ClaimsPrincipal user)
        {
            var guidTen = user.Claims?.Where(k => k.Type.Contains("TenantId") || k.Type.Contains("tenant-id"))?.FirstOrDefault()?.Value;

            if (String.IsNullOrEmpty(guidTen))
                return null;
            else
                return new Guid(guidTen);
        }
        /// <summary>
        /// Retrieve Organization Id claim from Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid? GetOrganizationId(this System.Security.Claims.ClaimsPrincipal user)
        {
            var guidOrg = user.Claims?.Where(k => k.Type.Contains("OrganizationId") || k.Type.Contains("organization-id"))?.FirstOrDefault()?.Value;

            if (String.IsNullOrEmpty(guidOrg))
                return null;
            else
                return new Guid(guidOrg);
        }

        /// <summary>
        /// Retrieve Organization Id claim from Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool CheckIfGodUser(this System.Security.Claims.ClaimsPrincipal user)
        {
            var isGodUser = user.Claims?.Where(k => k.Type.Contains("GodUser") || k.Type.Contains("god-user"))?.FirstOrDefault()?.Value;

            if (String.IsNullOrEmpty(isGodUser))
                return false;
            else if (Boolean.TryParse(isGodUser, out bool isGodUserBool))
                return isGodUserBool;
            else
                return false;
        }

        /// <summary>
        /// Retrieve Solution Filter List from Header
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Guid> GetSolutionFilter(this ActionExecutingContext context)
        {
            List<Guid> solutionList = new List<Guid>();

            StringValues sv = new StringValues();

            if (context.HttpContext.Request.Headers.TryGetValue("p360-solution-id", out sv))
            {
                var rawSolutions = sv.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(rawSolutions))
                {
                    return null;
                }

                var solutions = rawSolutions.Split(',').ToList();
                
                solutions.ForEach(k =>
                {
                    var cleaned = k.Trim().Trim('[', ']', '"'); 

                    if (Guid.TryParse(cleaned, out var guid)) 
                    {
                        solutionList.Add(guid);
                    }
                });
                
                return solutionList.Count > 0 ? solutionList : null;
            }
            return null;
        }
    }

}
