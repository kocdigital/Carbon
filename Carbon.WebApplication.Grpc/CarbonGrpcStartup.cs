using Carbon.Common;
using Carbon.WebApplication.TenantManagementHandler.Services;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Carbon.WebApplication.Grpc
{
    public abstract class CarbonGrpcStartup<TStartup> where TStartup : class
    {
        private bool _useAuthentication;
        private bool _useAuthorization;
        private CorsPolicySettings _corsPolicySettings;
        private string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        /// <summary>
        /// Provides information about the web hosting environment an application is running in.
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// Represents a set of key/value application configuration properties.
        /// </summary>
        public IConfiguration Configuration { get; }
        protected IList<FilterDescriptor> _filterDescriptors = new List<FilterDescriptor>();

        /// <summary>
        /// Adds operation filter.
        /// </summary>
        /// <typeparam name="T">Specifies the type of filter.</typeparam>
        /// <param name="args">Arguments of the filter</param>
        public void AddOperationFilter<T>(params object[] args) where T : IOperationFilter
        {
            _filterDescriptors.Add(new FilterDescriptor()
            {
                Type = typeof(T),
                Arguments = args
            });
        }

        /// <summary>
        /// Constructor that initializes configuration and environment variables.
        /// </summary>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        /// <param name="environment">Provides information about the web hosting environment an application is running in.</param>
        protected CarbonGrpcStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        /// Constructor that initializes configuration, environment, useAuthentication variables.
        /// </summary>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        /// <param name="environment">Provides information about the web hosting environment an application is running in.</param>
        /// <param name="useAuthentication">Indicates that use authentication or not</param>
        protected CarbonGrpcStartup(IConfiguration configuration, IWebHostEnvironment environment, bool useAuthentication)
        {
            Configuration = configuration;
            Environment = environment;
            _useAuthentication = useAuthentication;
        }

        /// <summary>
        /// Constructor that initializes configuration, environment, useAuthentication and useAuthorization variables.
        /// </summary>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        /// <param name="environment">Provides information about the web hosting environment an application is running in.</param>
        /// <param name="useAuthentication">Indicates that use authentication or not</param>
        /// <param name="useAuthorization">Indicates that use authorization or not</param>
        protected CarbonGrpcStartup(IConfiguration configuration, IWebHostEnvironment environment, bool useAuthentication, bool useAuthorization)
        {
            Configuration = configuration;
            Environment = environment;
            _useAuthentication = useAuthentication;
            _useAuthorization = useAuthorization;
        }

        /// <summary>
        /// Decides and Configures the services at startup. 
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("Carbon starting with .Net 5.0 with GRPC");
            services.AddGrpc();
            if (_useAuthorization)
            {
                services.AddAuthorization();
            }
            services.AddHeaderPropagation();

            services.AddOptions();
            services.Configure<SerilogSettings>(Configuration.GetSection("Serilog"));
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddHttpClient();

            services.AddSingleton(Configuration);
            services.AddScoped<IExternalService, ExternalService>();
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            #region Serilog Settings

            var _serilogSettings = Configuration.GetSection("Serilog").Get<SerilogSettings>();

            if (_serilogSettings == null)
                throw new ArgumentNullException("Serilog settings cannot be empty!");

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();

            #endregion
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

                            if (_corsPolicySettings.AllowAnyHeaders)
                            {
                                builder = builder.AllowAnyHeader();
                            }
                            builder.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
                        });
                });

            }

            #endregion


            services.AddHealthChecks();
            

            CustomConfigureServices(services);
        }

        /// <summary>
        /// Configures the application builder according to given environment
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHeaderPropagation();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseStaticFiles();
            app.UseRouting();

            CustomConfigure(app, env);
            app.UseGrpcWeb();
            if (_useAuthentication)
            {
                app.UseAuthentication();
            }
            if (_useAuthorization)
            {
                app.UseAuthorization();
            }

            if (_corsPolicySettings != null && (_corsPolicySettings.AllowAnyOrigin || (_corsPolicySettings.Origins != null && _corsPolicySettings.Origins.Count > 0)))
            {
                app.UseCors(MyAllowSpecificOrigins);
            }

            app.UseEndpoints(endpoints =>
            {
                GrpcConfigureServices(endpoints);
                endpoints.MapHealthChecks("/health");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }


        /// <summary>
        /// A Custom abstract method that configures the services. 
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        public abstract void CustomConfigureServices(IServiceCollection services);

        /// <summary>
        ///  A Custom abstract method that configures the application builder according to given environment
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public abstract void CustomConfigure(IApplicationBuilder app, IWebHostEnvironment env);


        public abstract void GrpcConfigureServices(IEndpointRouteBuilder endpoints);

    }
}