using Carbon.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Winton.Extensions.Configuration.Consul;

namespace Carbon.ConsoleApplication
{
    public static class IHostBuilderExtensions
    {
        /// <summary>
        /// Configures given <see cref="IHostBuilder"/> with given configurations
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/></typeparam>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureApp">Injects action for user defined configurations</param>
        /// <param name="configureServices">Injects action for user defined service configuraitons. <see cref="IHostBuilder.ConfigureServices"/></param>
        /// <returns>Configured <paramref name="builder"/></returns>
        private static IHostBuilder UseFeatures<TProgram>(IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureApp, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            var assemblyName = typeof(TProgram).Assembly.GetName().Name;
            var currentEnviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var consulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");

            builder.ConfigureAppConfiguration((h, c) =>
            {
                c.AddJsonFile($"appsettings.{currentEnviroment}.json", optional: true);
                c.AddEnvironmentVariables();

                #region Consul Configuration

                var consulEnabled = !string.IsNullOrEmpty(consulAddress);

                if (consulEnabled)
                {
                    c.AddConsul(
                                $"{assemblyName}/{currentEnviroment}", (options) =>
                                {
                                    options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulAddress); };
                                    options.Optional = false;
                                    options.ReloadOnChange = true;
                                    options.OnLoadException = exceptionContext => { exceptionContext.Ignore = false; };
                                });
                }

                #endregion

                var configuration = c.Build();

                var _serilogSettings = configuration.GetSection("Serilog").Get<SerilogSettings>();

                if (_serilogSettings == null)
                    throw new ArgumentNullException("Serilog settings cannot be empty!");

                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

                configureApp?.Invoke(h, c);

            }).ConfigureServices((h, s) =>
            {
                configureServices?.Invoke(h, s);
            });

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            return builder.UseSerilog();
        }

        /// <summary>
        /// Event handler for <see cref="AppDomain.CurrentDomain.UnhandledException"/>. Adds Log as Error
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="e"><see cref="UnhandledExceptionEventArgs"/></param>
        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Logger.Error("Unhandled exception occured!", e);
        }
        /// <summary>
        /// Use for configuring given <see cref="IHostBuilder"/> with Carbon Standards. 
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/</typeparam>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureServices">Injects action for user defined service configuraitons. <see cref="IHostBuilder.ConfigureServices"/></param>
        /// <returns>Configured <paramref name="builder"/></returns>
        /// <remarks>Carbon adds Consul and Serilog support.</remarks>
        public static IHostBuilder AddCarbonServices<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            return UseFeatures<TProgram>(builder, null, configureServices);
        }
        /// <summary>
        /// Configures <paramref name="builder"/> with given <paramref name="configureServiceProviders"/>
        /// </summary>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureServiceProviders"><see cref="Action"/> for configuring services. <seealso cref="IHostBuilder.ConfigureServices(Action{HostBuilderContext, IServiceCollection})"/></param>
        /// <returns>Given <paramref name="builder"/></returns>
        public static IHostBuilder UseServiceProvider(this IHostBuilder builder, Action<IServiceProvider> configureServiceProviders)
        {
            builder.ConfigureServices((h, s) =>
            {
                var serviceProvider = s.BuildServiceProvider();
                configureServiceProviders(serviceProvider);
            });

            return builder;
        }
        /// <summary>
        /// Use for configuring given <see cref="IHostBuilder"/> with Carbon Standards. 
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/</typeparam>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureApp">Injects action for user defined configuraitons.</param>
        /// <returns>Configured <paramref name="builder"/></returns>
        /// <remarks>Carbon adds Consul and Serilog support.</remarks>
        public static IHostBuilder AddCarbonConfiguration<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureApp) where TProgram : class
        {
            return UseFeatures<TProgram>(builder, configureApp, null);
        }

    }
}
