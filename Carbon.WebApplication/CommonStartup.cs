﻿using Carbon.Common;
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
using Serilog.Enrichers.Sensitive;
using Carbon.Serilog;

namespace Carbon.WebApplication
{
    internal static class CommonStartup
    {
        internal static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        internal static SwaggerSettings _swaggerSettings;
        internal static CorsPolicySettings _corsPolicySettings;
        internal static IList<FilterDescriptor> _filterDescriptors = new List<FilterDescriptor>();
        private static bool _useAuthentication;
        private static bool _useAuthorization;
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

        internal static void AddServiceBaseLogic(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddHeaderPropagation();

            services.AddOptions();
            services.AddControllers();
            services.Configure<SerilogSettings>(Configuration.GetSection("Serilog"));
            services.Configure<CorsPolicySettings>(Configuration.GetSection("CorsPolicy"));
            services.Configure<SwaggerSettings>(Configuration.GetSection("Swagger"));
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddHttpClient();

            services.AddSingleton(Configuration);
            services.AddScoped<IExternalService, ExternalService>();
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
            #region Serilog Settings

            Log.Logger = SerilogExtensions.CreateLogger(Configuration);


            #endregion

            AddServiceCors(services, Configuration);

            services.AddHealthChecks();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelFilter));
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddJsonOptions(options =>
            {

#if NET6_0
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
#elif NET5_0 || NETCOREAPP3_1
                options.JsonSerializerOptions.IgnoreNullValues = true;
#endif       
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }).AddHybridModelBinder();

            services.AddFluentValidation(fv =>
            {
                fv.DisableDataAnnotationsValidation = true;
                fv.RegisterValidatorsFromAssembly(System.Reflection.Assembly.GetEntryAssembly());
            });

            var apiname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddHeaderPropagation();
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
                                builder = builder.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins(_corsPolicySettings.Origins.ToArray());
                            }

                            if (_corsPolicySettings.AllowAnyMethods)
                            {
                                builder = builder.AllowAnyMethod();
                            }

                            if (_corsPolicySettings.AllowAnyHeaders)
                            {
                                builder = builder.AllowAnyHeader();
                            }

                            if (_corsPolicySettings.AllowCredentials)
                            {
                                builder = builder.SetIsOriginAllowed(origin => true).AllowCredentials();
                            }

                            if (_corsPolicySettings.ExposePaginationHeaders)
                            {
                                builder = builder.WithExposedHeaders(
                                    "X-Paging-PageIndex",
                                    "X-Paging-PageSize",
                                    "X-Paging-PageCount",
                                    "X-Paging-TotalRecordCount",
                                    "X-Paging-Previous-Link",
                                    "X-Paging-Next-Link",
                                    "X-CorrelationId",
                                    "Location"
                                    );
                            }
                        });
                });
            }

            #endregion
            
        }

        public static void AddServiceSwagger(IServiceCollection services, IConfiguration Configuration)
        {
            #region Swagger Settings

            _swaggerSettings = Configuration.GetSection("Swagger").Get<SwaggerSettings>();

            if (_swaggerSettings == null)
                throw new ArgumentNullException("Swagger settings cannot be empty!");

            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<HeaderParameterExtension>();
                c.OperationFilter<HybridOperationFilter>();
                c.OperationFilterDescriptors.AddRange(_filterDescriptors);
                c.CustomSchemaIds(x => x.FullName);
                c.AddServer(new OpenApiServer()
                {
                    Url = _swaggerSettings.EndpointUrl
                });
                foreach (var doc in _swaggerSettings.Documents)
                {
                    c.SwaggerDoc(doc.DocumentName, new OpenApiInfo { Title = doc.OpenApiInfo.Title, Version = doc.OpenApiInfo.Version, Description = doc.OpenApiInfo.Description });
                    if (_swaggerSettings.EnableXml)
                    {
                        var xmlFile = $"{_swaggerSettings.EndpointName}.xml";
                        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                        c.IncludeXmlComments(xmlPath);
                        c.EnableAnnotations();
                    }
                    if (doc.Security != null)
                    {
                        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                Implicit = new OpenApiOAuthFlow()
                                {
                                    AuthorizationUrl = new Uri(doc.Security.AuthorizationUrl + "/connect/authorize", UriKind.Absolute),
                                    Scopes = doc.Security.Scopes?.ToDictionary(x => x.Key, x => x.Description)
                                }
                            }
                        });

                        c.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                                },
                                doc.Security.Scopes.Select(x => x.Key).ToArray()
                            }
                        });
                    }
                }
            });

            #endregion

        }

        internal static void AddAppBase(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHeaderPropagation();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.OAuth2RedirectUrl($"{_swaggerSettings.EndpointUrl}/swagger/oauth2-redirect.html");
                c.SwaggerEndpoint(_swaggerSettings.RoutePrefix + _swaggerSettings.EndpointPath, _swaggerSettings.EndpointName);
            });

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
            if (_corsPolicySettings != null && (_corsPolicySettings.AllowAnyOrigin || (_corsPolicySettings.Origins != null && _corsPolicySettings.Origins.Count > 0)))
            {
                app.UseCors(MyAllowSpecificOrigins);
            }
            
            _useAuthentication = useAuthentication;
            _useAuthorization = useAuthorization;
            if (_useAuthentication)
            {
                app.UseAuthentication();
            }

            if (_useAuthorization)
            {
                app.UseAuthorization();
            }
            
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
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
        }

    }
}
