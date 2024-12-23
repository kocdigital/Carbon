#if (NET6_0_OR_GREATER || NET8_0_OR_GREATER)
using Carbon.Common;
using Carbon.WebApplication.TenantManagementHandler.Extensions;
using Carbon.WebApplication.TenantManagementHandler.Services;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.WebApplication
{
    public static class CarbonMinimalStartup
    {
        private static bool _useAuthentication;
        private static bool _useAuthorization;
        /// <summary>
        /// Represents a set of key/value application configuration properties.
        /// </summary>
        public static IConfiguration Configuration { get; set; }


        public static void AddCarbonServices(this WebApplicationBuilder builder, Func<IServiceCollection, IServiceCollection> serviceCollector)
        {
            builder.WebHost.UseCarbonFeatures();
            var services = builder.Services;
            Configuration = builder.Configuration;
#if NET8_0
            Console.WriteLine($"Carbon is compiled with .Net 8.0 and working on .Net {System.Environment.Version.ToString()}. (Minimal api)");
#elif NET6_0       
            Console.WriteLine($"Carbon is compiled with .Net 6.0 and working on .Net {System.Environment.Version.ToString()}. (Minimal api)");
#elif NET5_0       
            Console.WriteLine($"Carbon is compiled with .Net 5.0 and working on .Net {System.Environment.Version.ToString()}. (Minimal api)");
#elif NETCOREAPP3_1
            Console.WriteLine($"Carbon is compiled with .NetCore 3.1 and working on .Net {System.Environment.Version.ToString()}. (Minimal api)");
#endif
            CommonStartup.AddServiceBaseLogic(services, Configuration);

            services = serviceCollector(services);

            CommonStartup.AddServiceSwagger(services, Configuration);
        }

        public static void AddCarbonApplication(this Microsoft.AspNetCore.Builder.WebApplication app, Func<Microsoft.AspNetCore.Builder.WebApplication, Microsoft.AspNetCore.Builder.WebApplication> applicationCollector, bool useAuthentication = true, bool useAuthorization = true, Func<IEndpointRouteBuilder, IEndpointRouteBuilder> endpointRouteCollector = null)
        {
            _useAuthentication = useAuthentication;
            _useAuthorization = useAuthorization;
            var env = app.Environment;
            CommonStartup.AddAppBase(app, env);

            app = applicationCollector(app);

            CommonStartup.AddAppAuths(app, _useAuthentication, _useAuthorization);

            app.UseEndpoints(endpoints =>
            {
                if (endpointRouteCollector != null)
                    endpointRouteCollector(endpoints);

                CommonStartup.AddAppEndpoints(endpoints);
            });
        }
    }
}
#endif