using Carbon.Common;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Linq;

namespace Carbon.WebApplication
{
    public abstract class CarbonStartup<TStartup> where TStartup : class
    {
        private string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private SwaggerSettings _swaggerSettings;
        private bool _useAuthentication;
        private bool _useAutherization;
        private bool v;

        protected CarbonStartup(IConfiguration configuration, bool useAuthentication, bool useAutherization)
        {
            Configuration = configuration;
            _useAuthentication = useAuthentication;
            _useAutherization = useAutherization;
        }

        protected CarbonStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected CarbonStartup(IConfiguration configuration, bool v)
        {
            Configuration = configuration;
            this.v = v;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddControllers();
            services.Configure<SerilogSettings>(Configuration.GetSection("Serilog"));
            services.Configure<CorsPolicySettings>(Configuration.GetSection("CorsPolicy"));
            services.Configure<SwaggerSettings>(Configuration.GetSection("Swagger"));
            services.AddSingleton(Configuration);

            #region Serilog Settings

            var _serilogSettings = Configuration.GetSection("Serilog").Get<SerilogSettings>();

            if (_serilogSettings == null)
                throw new ArgumentNullException("Serilog settings cannot be empty!");
 
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
       
            #endregion

            #region Swagger Settings

            _swaggerSettings = Configuration.GetSection("Swagger").Get<SwaggerSettings>();

            if (_swaggerSettings == null)
                throw new ArgumentNullException("Swagger settings cannot be empty!");

            services.AddSwaggerGen(c =>
            {
                foreach (var doc in _swaggerSettings.Documents)
                {
                    c.SwaggerDoc(doc.DocumentName, new OpenApiInfo { Title = doc.OpenApiInfo.Title, Version = doc.OpenApiInfo.Version, Description = doc.OpenApiInfo.Description });
                }
            });

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
            }).AddFluentValidation(fv => {
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                fv.RegisterValidatorsFromAssemblyContaining<TStartup>();
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            ConfigureDependencies(services);
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

            if (_useAutherization)
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
