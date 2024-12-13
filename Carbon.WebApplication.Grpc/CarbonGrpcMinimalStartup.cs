#if NET6_0
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

namespace Carbon.WebApplication.Grpc
{
    public static class CarbonGrpcMinimalStartup
    {
        private static bool _useAuthentication;
        private static bool _useAuthorization;
        /// <summary>
        /// Represents a set of key/value application configuration properties.
        /// </summary>
        public static IConfiguration Configuration { get; set; }


        public static void AddCarbonGrpcServices(this WebApplicationBuilder builder, Func<IServiceCollection, IServiceCollection> serviceCollector, bool useAuthentication = true, bool useAuthorization = true, List<Type> interceptors = null)
        {
            _useAuthorization = useAuthorization;
            _useAuthentication = useAuthentication;
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
            CommonStartup.AddServiceBaseLogic(services, Configuration, _useAuthorization, interceptors);

            serviceCollector(services);
            builder.WebHost.UseCarbonFeatures();
        }

        public static void AddCarbonGrpcApplication(this Microsoft.AspNetCore.Builder.WebApplication app, Func<Microsoft.AspNetCore.Builder.WebApplication, Microsoft.AspNetCore.Builder.WebApplication> applicationCollector, Func<IEndpointRouteBuilder, IEndpointRouteBuilder> endpointRouteCollector = null)
        {
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