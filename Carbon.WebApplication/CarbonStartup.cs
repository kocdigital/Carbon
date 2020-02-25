using Carbon.Common;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Carbon.WebApplication
{
    public abstract class CarbonStartup<TStartup> where TStartup : class
    {
        private string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private SwaggerSettings _swaggerSettings;
        private bool _useAuthentication;
        private bool _useAuthorization;

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        protected IList<FilterDescriptor> _filterDescriptors = new List<FilterDescriptor>();

        public void AddOperationFilter<T>(params object[] args) where T : IOperationFilter
        {
            _filterDescriptors.Add(new FilterDescriptor()
            {
                Type = typeof(T),
                Arguments = args
            });
        }

        protected CarbonStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        protected CarbonStartup(IConfiguration configuration, IWebHostEnvironment environment, bool useAuthentication)
        {
            Configuration = configuration;
            Environment = environment;
            _useAuthentication = useAuthentication;
        }
        protected CarbonStartup(IConfiguration configuration, IWebHostEnvironment environment, bool useAuthentication, bool useAuthorization)
        {
            Configuration = configuration;
            Environment = environment;
            _useAuthentication = useAuthentication;
            _useAuthorization = useAuthorization;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddControllers();
            services.Configure<SerilogSettings>(Configuration.GetSection("Serilog"));
            services.Configure<CorsPolicySettings>(Configuration.GetSection("CorsPolicy"));
            services.Configure<SwaggerSettings>(Configuration.GetSection("Swagger"));
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddSingleton(Configuration);

            #region Serilog Settings

            var _serilogSettings = Configuration.GetSection("Serilog").Get<SerilogSettings>();

            if (_serilogSettings == null)
                throw new ArgumentNullException("Serilog settings cannot be empty!");

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();

            #endregion

            #region Cors Policy Settings

            var corsSettings = Configuration.GetSection("CorsPolicy").Get<CorsPolicySettings>();

            if (corsSettings != null && corsSettings.Origins != null && corsSettings.Origins.Count > 0)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins(corsSettings.Origins.ToArray())
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
                });

            }

            #endregion

            services.AddHealthChecks();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelFilter));
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddJsonOptions(
            options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            }).AddFluentValidation(fv =>
            {
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                fv.RegisterValidatorsFromAssemblyContaining<TStartup>();
            }).AddHybridModelBinder();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            ConfigureDependencies(services);

            #region Swagger Settings

            _swaggerSettings = Configuration.GetSection("Swagger").Get<SwaggerSettings>();

            if (_swaggerSettings == null)
                throw new ArgumentNullException("Swagger settings cannot be empty!");

            services.AddSwaggerGen(c =>
            {
                //c.OperationFilter<HybridOperationFilter>();
                c.OperationFilterDescriptors.AddRange(_filterDescriptors);

                foreach (var doc in _swaggerSettings.Documents)
                {
                    c.SwaggerDoc(doc.DocumentName, new OpenApiInfo { Title = doc.OpenApiInfo.Title, Version = doc.OpenApiInfo.Version, Description = doc.OpenApiInfo.Description });

                    if (doc.Security != null)
                    {
                        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                Implicit = new OpenApiOAuthFlow()
                                {
                                    AuthorizationUrl = new Uri(_swaggerSettings.EndpointAddress + "/connect/authorize", UriKind.Absolute),
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(_swaggerSettings.EndpointUrl, _swaggerSettings.EndpointName);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseStaticFiles();
            app.UseRouting();

            ConfigureRequestPipeline(app, env);

            if (_useAuthentication)
            {
                app.UseAuthentication();
            }

            if (_useAuthorization)
            {
                app.UseAuthorization();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }

        public abstract void ConfigureDependencies(IServiceCollection services);
        public abstract void ConfigureRequestPipeline(IApplicationBuilder app, IWebHostEnvironment env);
    }
}
