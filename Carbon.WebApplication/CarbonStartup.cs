using Carbon.Common;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        protected CarbonStartup(IConfiguration configuration)
        {
            Configuration = configuration;
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

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<TStartup>());

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

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureRequestPipeline(app, env);
        }

        public abstract void ConfigureDependencies(IServiceCollection services);
        public abstract void ConfigureRequestPipeline(IApplicationBuilder app, IWebHostEnvironment env);
    }
}
