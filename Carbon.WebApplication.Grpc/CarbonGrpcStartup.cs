using Carbon.Common;
using Carbon.WebApplication.Grpc.Interceptors;
using Carbon.WebApplication.TenantManagementHandler.Services;
using HealthChecks.UI.Client;
using Mapster;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
        private List<Type> _interceptors;
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
        protected CarbonGrpcStartup(IConfiguration configuration, IWebHostEnvironment environment, List<Type> interceptors = null)
        {
            Configuration = configuration;
            Environment = environment;
            _interceptors = interceptors ?? new List<Type>();
        }

        /// <summary>
        /// Constructor that initializes configuration, environment, useAuthentication variables.
        /// </summary>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        /// <param name="environment">Provides information about the web hosting environment an application is running in.</param>
        /// <param name="useAuthentication">Indicates that use authentication or not</param>
        protected CarbonGrpcStartup(IConfiguration configuration, IWebHostEnvironment environment, bool useAuthentication, List<Type> interceptors = null)
        {
            Configuration = configuration;
            Environment = environment;
            _useAuthentication = useAuthentication;
            _interceptors = interceptors ?? new List<Type>();
        }

        /// <summary>
        /// Constructor that initializes configuration, environment, useAuthentication and useAuthorization variables.
        /// </summary>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        /// <param name="environment">Provides information about the web hosting environment an application is running in.</param>
        /// <param name="useAuthentication">Indicates that use authentication or not</param>
        /// <param name="useAuthorization">Indicates that use authorization or not</param>
        protected CarbonGrpcStartup(IConfiguration configuration, IWebHostEnvironment environment, bool useAuthentication, bool useAuthorization, List<Type> interceptors = null)
        {
            Configuration = configuration;
            Environment = environment;
            _useAuthentication = useAuthentication;
            _useAuthorization = useAuthorization;
            _interceptors = interceptors ?? new List<Type>();
        }

        /// <summary>
        /// Decides and Configures the services at startup. 
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
#if NET8_0
            Console.WriteLine($"Carbon is compiled with .Net 8.0 with GRPC and working on .Net {System.Environment.Version.ToString()}. (not minimal api)");
#elif NET6_0
			Console.WriteLine($"Carbon is compiled with .Net 6.0 with GRPC and working on .Net {System.Environment.Version.ToString()}.");
#elif NET5_0
			Console.WriteLine($"Carbon is compiled with .Net 5.0 with GRPC and working on .Net {System.Environment.Version.ToString()}.");
#elif NETCOREAPP3_1
            Console.WriteLine($"Carbon is compiled with .NetCore 3.1 with GRPC and working on .Net {System.Environment.Version.ToString()}.");
#endif
            CommonStartup.AddServiceBaseLogic(services, Configuration, _useAuthorization, _interceptors);
            CustomConfigureServices(services);
        }

        /// <summary>
        /// Configures the application builder according to given environment
        /// </summary>
        /// <param name="app">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            CommonStartup.AddAppBase(app, env);

            CustomConfigure(app, env);

            CommonStartup.AddAppAuths(app, _useAuthentication, _useAuthorization);

            app.UseEndpoints(endpoints =>
            {
                GrpcConfigureServices(endpoints);
                CommonStartup.AddAppEndpoints(endpoints);
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