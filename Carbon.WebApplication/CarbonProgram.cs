using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Winton.Extensions.Configuration.Consul;

namespace Carbon.WebApplication
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder AddCarbonFeatures(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureWebHost(webBuilder =>
            {
                var assemblyName = typeof(Host).Assembly.GetName().Name;
                var currentEnviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var consulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");

                webBuilder.ConfigureAppConfiguration((c) =>
                {
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
                });

                webBuilder.UseSerilog();

            });

            return hostBuilder;
        }
    }

}
