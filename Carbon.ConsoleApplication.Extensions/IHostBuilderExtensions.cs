using Carbon.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;
using Winton.Extensions.Configuration.Consul;

namespace Carbon.ConsoleApplication
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder UseCarbonFeatures<TProgram>(this IHostBuilder builder) where TProgram : class
        {
            var assemblyName = typeof(TProgram).Assembly.GetName().Name;
            var currentEnviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var consulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");

            builder.ConfigureAppConfiguration((c, x) =>
            {
                x.AddJsonFile($"appsettings.{currentEnviroment}.json", optional: true);
                x.AddEnvironmentVariables();

                #region Consul Configuration

                var consulEnabled = !string.IsNullOrEmpty(consulAddress);

                if (consulEnabled)
                {
                    x.AddConsul(
                                $"{assemblyName}/{currentEnviroment}", (options) =>
                                {
                                    options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulAddress); };
                                    options.Optional = false;
                                    options.ReloadOnChange = true;
                                    options.OnLoadException = exceptionContext => { exceptionContext.Ignore = false; };
                                });
                }

                #endregion

                var configuration = x.Build();

                var _serilogSettings = configuration.GetSection("Serilog").Get<SerilogSettings>();

                if (_serilogSettings == null)
                    throw new ArgumentNullException("Serilog settings cannot be empty!");

                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(c.Configuration).CreateLogger();
            });

            return builder.UseSerilog();
        }
    }
}
