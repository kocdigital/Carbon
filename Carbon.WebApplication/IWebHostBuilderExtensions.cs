using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Linq;
using System.Reflection;
using Winton.Extensions.Configuration.Consul;

namespace Carbon.WebApplication
{
    public static class IWebHostBuilderExtensions
    {
        public static void UseCarbonFeatures<TStartup>(this IWebHostBuilder builder) where TStartup : class
        {
            var assemblyName = typeof(TStartup).Assembly.GetName().Name;
            var currentEnviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var consulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");

            var consulKeys = Environment.GetEnvironmentVariable(assemblyName + "_CONSUL_KEYS");

            builder.ConfigureAppConfiguration((c) =>
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

                    if (!string.IsNullOrEmpty(consulKeys))
                    {

                        var array = consulKeys.Split(',').ToArray();
                        foreach (var additionalConsulKey in array)
                        {
                            c.AddConsul(
                               $"{additionalConsulKey}/{currentEnviroment}", (options) =>
                               {
                                   options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulAddress); };
                                   options.Optional = false;
                                   options.ReloadOnChange = true;
                                   options.OnLoadException = exceptionContext => { exceptionContext.Ignore = false; };
                               });
                        }
                    }
                }



                #endregion
            });


            builder.UseSerilog();
        }

    }

}
