using Carbon.Common;
using Carbon.Serilog;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Enrichers.Sensitive;

using System;
using System.IO;
using System.Linq;

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
        /// <param name="configureServices">Injects action for user defined service configurations. <see cref="IHostBuilder.ConfigureServices"/></param>
        /// <returns>Configured <paramref name="builder"/></returns>
        private static IHostBuilder UseFeatures<TProgram>(IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureApp, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            var assemblyName = typeof(TProgram).Assembly.GetName().Name;
            var currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var consulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");
            var consulKeysValue = Environment.GetEnvironmentVariable(assemblyName + "_CONSUL_KEYS");
            var confType = Environment.GetEnvironmentVariable("CONFIGURATION_TYPE");

            builder.ConfigureAppConfiguration((h, c) =>
            {
                c.AddEnvironmentVariables();

                #region Consul Configuration
                if (string.IsNullOrEmpty(confType) || string.Equals(confType, "CONSUL", StringComparison.InvariantCultureIgnoreCase))
                {
                    var consulEnabled = !string.IsNullOrEmpty(consulAddress);

                    if (consulEnabled)
                    {
                        Console.WriteLine("Configuration Type: CONSUL");
                        if (!string.IsNullOrEmpty(consulKeysValue))
                        {
                            var consulKeys = consulKeysValue.Split(',').ToArray();
                            foreach (var consulKey in consulKeys)
                            {
                                c.AddConsul(
                                   $"{consulKey}/{currentEnvironment}", (options) =>
                                   {
                                       options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulAddress); };
                                       options.Optional = false;
                                       options.ReloadOnChange = true;
                                       options.OnLoadException = exceptionContext => { exceptionContext.Ignore = false; };
                                   });
                            }
                        }
                        else
                        {
                            c.AddConsul(
                                        $"{assemblyName}/{currentEnvironment}", (options) =>
                                        {
                                            options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulAddress); };
                                            options.Optional = false;
                                            options.ReloadOnChange = true;
                                            options.OnLoadException = exceptionContext => { exceptionContext.Ignore = false; };
                                        });
                        }
                    }
                }
                else if (string.Equals(confType, "FILE", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Configuration Type: FILE");
                    var kubConfigPath = Environment.GetEnvironmentVariable("FILE_CONFIG_PATHS") ?? "config/appsettings.main.file.json";
                    Console.WriteLine("Config Paths => " + kubConfigPath);

                    var kubConfigPaths = kubConfigPath.Split(',').ToArray();

                    foreach (var kubCnf in kubConfigPaths)
                    {
                        Console.WriteLine("Adding Config =>  " + kubCnf);
                        try
                        {
                            var configToRead = File.ReadAllText(kubCnf);
#if DEBUG
                            Console.WriteLine("Inserting Config => \n" + configToRead);
#endif
                        }
                        catch
                        {
                            Console.WriteLine("Config File not found! No configurations may be loaded!");
                        }
                        c.AddJsonFile(kubCnf, optional: true, reloadOnChange: true);
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown Configuration Source Specified! Defaults to => appsettings.{currentEnvironment}.json");
                    c.AddJsonFile($"appsettings.{currentEnvironment}.json", optional: true);
                }
                #endregion

                var configuration = c.Build();

                Log.Logger = SerilogExtensions.CreateLogger(configuration);

                configureApp?.Invoke(h, c);

            }).ConfigureServices((h, s) =>
            {
                configureServices?.Invoke(h, s);
            });

            
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            return builder.UseSerilog(); 
        }

        /// <summary>
        /// Event handler for <see cref="AppDomain.UnhandledException"/>. Adds Log as Error
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="e"><see cref="UnhandledExceptionEventArgs"/></param>
        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                var ex = e.ExceptionObject as Exception;
                Log.Logger.Error("Unhandled exception occured! {0}: {1}\n {2}", e.ExceptionObject.GetType(), ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Use for configuring given <see cref="IHostBuilder"/> with Carbon Standards. 
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/></typeparam>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureServices">Injects action for user defined service configurations. <see cref="IHostBuilder.ConfigureServices"/></param>
        /// <returns>Configured <paramref name="builder"/></returns>
        /// <remarks>Carbon adds Consul and Serilog support.</remarks>
        public static IHostBuilder AddCarbonServices<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class => UseFeatures<TProgram>(builder, null, configureServices);

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
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/></typeparam>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureApp">Injects action for user defined configurations.</param>
        /// <returns>Configured <paramref name="builder"/></returns>
        /// <remarks>Carbon adds Consul and Serilog support.</remarks>
        public static IHostBuilder AddCarbonConfiguration<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureApp) where TProgram : class => UseFeatures<TProgram>(builder, configureApp, null);
    }
}
