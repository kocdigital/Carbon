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

                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

                configureApp?.Invoke(h, c);

            }).ConfigureServices((h, s) =>
            {
                configureServices?.Invoke(h, s);
            });

            return builder.UseSerilog();
        }

        public static IHostBuilder UseCarbonConfigureServices<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            return UseFeatures<TProgram>(builder, null, configureServices);
        }

        public static IHostBuilder UseCarbonConfigureApplication<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureApp) where TProgram : class
        {
            return UseFeatures<TProgram>(builder, configureApp, null);
        }

    }
}
