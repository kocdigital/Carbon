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

                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(h.Configuration).CreateLogger();

                configureApp?.Invoke(h, c);

            }).ConfigureServices((h, s) =>
            {
                configureServices?.Invoke(h, s);
            });

            return builder.UseSerilog();
        }

        //
        // Summary:
        //     Add app.{}.settings, consul and serilog by default 
        public static IHostBuilder UseCarbonFeatures<TProgram>(this IHostBuilder builder) where TProgram : class
        {
            return UseFeatures<TProgram>(builder, null, null);
        }

        //
        // Summary:
        //     Add app.{}.settings, consul and serilog by default and configure services
        public static IHostBuilder UseCarbonFeatures<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            return UseFeatures<TProgram>(builder, null, configureServices);
        }

        //
        // Summary:
        //     Add app.{}.settings, consul and serilog by default and configure applications
        public static IHostBuilder UseCarbonFeatures<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureApp) where TProgram : class
        {
            return UseFeatures<TProgram>(builder, configureApp, null);
        }

        //
        // Summary:
        //     Add app.{}.settings, consul and serilog by default and configure applications & services
        public static IHostBuilder UseCarbonFeatures<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureApp, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            return UseFeatures<TProgram>(builder, configureApp, configureServices);
        }
    }
}
