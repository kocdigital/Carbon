using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Winton.Extensions.Configuration.Consul;

namespace Carbon.WebApplication
{
    public abstract class CarbonProgram<TStartup> where TStartup : class
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var assemblyName = typeof(TStartup).Assembly.GetName().Name;
                    var currentEnviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var consulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");

                    webBuilder.ConfigureAppConfiguration((c) =>
                    {
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
                    });

                    webBuilder.UseStartup<TStartup>();
                    webBuilder.UseSerilog();
                });
    }
}
