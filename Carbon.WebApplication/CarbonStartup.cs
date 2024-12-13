using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.WebApplication
{
    /// <summary>
	/// A base class for Startup class. Helps to automatically configure your API with Carbon standarts
	/// </summary>
	/// <remarks>
	/// Configures Swagger, Serilog, Authentication, Authorization, Cors and HealthChecks with given Configuration options
	/// </remarks>
	/// <typeparam name="TStartup">
	/// Your Startup class
	/// <para>
	/// Use it like;
    /// <code>MyStartup : CarbonStartup&lt;MyStartUp&gt;</code>
	/// </para>
	/// </typeparam>
    public abstract class CarbonStartup<TStartup> where TStartup : class
    {
        private bool _useAuthentication;
        private bool _useAuthorization;
        /// <summary>
        /// Provides information about the web hosting environment an application is running in.
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// Represents a set of key/value application configuration properties.
        /// </summary>
        public IConfiguration Configuration { get; }

       

        /// <summary>
        /// Constructor that initializes configuration and environment variables.
        /// </summary>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        /// <param name="environment">Provides information about the web hosting environment an application is running in.</param>
        protected CarbonStartup(IConfiguration configuration, IWebHostEnvironment environment)
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
        protected CarbonStartup(IConfiguration configuration, IWebHostEnvironment environment, bool useAuthentication)
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
        protected CarbonStartup(IConfiguration configuration, IWebHostEnvironment environment, bool useAuthentication, bool useAuthorization)
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
#if NET8_0
            Console.WriteLine($"Carbon is compiled with .Net 8.0 and working on .Net {System.Environment.Version.ToString()}. (not minimal api)");
#elif NET6_0       
            Console.WriteLine($"Carbon is compiled with .Net 6.0 and working on .Net {System.Environment.Version.ToString()}.");
#elif NET5_0       
            Console.WriteLine($"Carbon is compiled with .Net 5.0 and working on .Net {System.Environment.Version.ToString()}.");
#elif NETCOREAPP3_1
            Console.WriteLine($"Carbon is compiled with .NetCore 3.1 and working on .Net {System.Environment.Version.ToString()}.");
#endif

            CommonStartup.AddServiceBaseLogic(services, Configuration);

            CustomConfigureServices(services);

            #region Swagger Settings
            CommonStartup.AddServiceSwagger(services, Configuration);
            #endregion
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
                ConfigureEndpoints(endpoints);
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

        public virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            CommonStartup.AddAppEndpoints(endpoints);
        }

    }
}
