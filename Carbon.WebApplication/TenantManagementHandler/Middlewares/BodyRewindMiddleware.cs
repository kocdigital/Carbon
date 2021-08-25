using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.WebApplication.TenantManagementHandler.Middlewares
{
    public sealed class BodyRewindMiddleware
    {
        private readonly RequestDelegate _next;

        public BodyRewindMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try { context.Request.EnableBuffering(); } catch { }
            await _next(context);
            // context.Request.Body.Dipose() might be added to release memory, not tested
        }
    }
    public static class BodyRewindExtensions
    {
        /// <summary>
        /// Enables Dto Validation in Carbon Tenant Management for RoleFilteredBaseDto based dto classes
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder EnableRequestBodyRewind(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            BodyRewindSettings.Enabled = true;
            return app.UseMiddleware<BodyRewindMiddleware>();
        }

    }

    public static class BodyRewindSettings
    {
        public static bool Enabled { get; set; }
    }
}
