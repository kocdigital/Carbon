using Carbon.Common;
using Carbon.WebApplication.TenantManagementHandler.Extensions;
using Carbon.WebApplication.TenantManagementHandler.Services;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Carbon.WebApplication.Grpc.Interceptors;
using Serilog.Enrichers.Sensitive;
using Carbon.Serilog;

namespace Carbon.WebApplication.Grpc
{
    internal static class CommonStartup
    {
        internal static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        internal static CorsPolicySettings _corsPolicySettings;
        internal static IList<FilterDescriptor> _filterDescriptors = new List<FilterDescriptor>();
        private static bool _useAuthentication;
        private static bool _useAuthorization;
        private static List<Type> _interceptors;

        /// <summary>
        /// Adds operation filter.
        /// </summary>
        /// <typeparam name="T">Specifies the type of filter.</typeparam>
        /// <param name="args">Arguments of the filter</param>
        internal static void AddOperationFilter<T>(params object[] args) where T : IOperationFilter
        {
            _filterDescriptors.Add(new FilterDescriptor()
            {
                Type = typeof(T),
                Arguments = args
            });
        }

        internal static void AddServiceBaseLogic(IServiceCollection services, IConfiguration Configuration, bool useAuthorization = true, List<Type> interceptors = null)
        {
            _useAuthorization = useAuthorization;
            _interceptors = interceptors;

            services.AddGrpc(options =>
            {
                options.Interceptors.Add<CarbonRequestDtoInterceptor>();
                if (_interceptors != null)
                    foreach (var interceptor in _interceptors)
                    {
                        options.Interceptors.Add(interceptor);
                    }
            });

            services.AddHeaderPropagation();

            services.AddOptions();
            services.Configure<SerilogSettings>(Configuration.GetSection("Serilog"));
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddHttpClient();

            services.AddSingleton(Configuration);
            services.AddScoped<IExternalService, ExternalService>();
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            #region Serilog Settings

            Log.Logger = SerilogExtensions.CreateLogger(Configuration);
            #endregion
            AddServiceCors(services, Configuration);
            if (_useAuthorization)
            {
                services.AddAuthorization();
            }

            services.AddHealthChecks();
        }
        private static void AddServiceCors(IServiceCollection services, IConfiguration Configuration)
        {
            #region Cors Policy Settings

            _corsPolicySettings = Configuration.GetSection("CorsPolicy").Get<CorsPolicySettings>();

            if (_corsPolicySettings != null)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(MyAllowSpecificOrigins,
                        builder =>
                        {
                            if (_corsPolicySettings.AllowAnyOrigin)
                            {
                                builder = builder.AllowAnyOrigin();
                            }
                            else if (_corsPolicySettings.Origins != null && _corsPolicySettings.Origins.Count > 0)
                            {
                                builder = builder.WithOrigins(_corsPolicySettings.Origins.ToArray());
                            }

                            if (_corsPolicySettings.AllowAnyMethods)
                            {
                                builder = builder.AllowAnyMethod();
                            }

                            if (_corsPolicySettings.AllowCredentials)
                            {
                                builder = builder.AllowCredentials();
                            }

                            if (_corsPolicySettings.AllowAnyHeaders)
                            {
                                builder = builder.AllowAnyHeader();
                            }
                            builder.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding", "Location");
                        });
                });

            }


            #endregion


        }

        internal static void AddAppBase(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHeaderPropagation();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseStaticFiles();
            app.UseRouting();
        }

        internal static void AddAppAuths(IApplicationBuilder app, bool useAuthentication, bool useAuthorization)
        {
            _useAuthentication = useAuthentication;
            _useAuthorization = useAuthorization;
            if (_corsPolicySettings != null && (_corsPolicySettings.AllowAnyOrigin || (_corsPolicySettings.Origins != null && _corsPolicySettings.Origins.Count > 0)))
            {
                app.UseCors(MyAllowSpecificOrigins);
            }

            if (_useAuthentication)
            {
                app.UseAuthentication();
            }
            if (_useAuthorization)
            {
                app.UseAuthorization();
            }
            app.UseGrpcWeb();
        }

        internal static void AddAppEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }

            });
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        }

    }
}
