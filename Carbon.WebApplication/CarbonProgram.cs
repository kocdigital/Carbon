using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                    var configurationBuilder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{currentEnviroment}.json", optional: true)
                        .AddEnvironmentVariables();

                    var consulEnabled = !string.IsNullOrEmpty(consulAddress);

                    if (consulEnabled)
                    {
                        configurationBuilder.AddConsul(
                            $"{assemblyName}/{currentEnviroment}", (options) =>
                            {
                                options.ConsulConfigurationOptions =
                                    cco => { cco.Address = new Uri(consulAddress); };
                                options.Optional = false;
                                options.ReloadOnChange = true;
                                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = false; };
                            });
                    }

                    var configuration = configurationBuilder.Build();

                    Log.Logger = new LoggerConfiguration()
                                        .ReadFrom.Configuration(configuration)
                                        .CreateLogger();

                    Log.Information($"Starting {assemblyName} - {currentEnviroment} - ConsulEnabled : {consulEnabled}");
              
                    webBuilder.UseStartup<TStartup>();
                    webBuilder.UseSerilog();
                });
    }
}
