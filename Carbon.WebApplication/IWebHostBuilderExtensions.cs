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
        /// <summary>
        /// Applies Carbon Framework settings such as Environment, Consul Address, Assembly Name etc.
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="builder"></param>
        public static void UseCarbonFeatures<TStartup>(this IWebHostBuilder builder) where TStartup : class
        {
            var assemblyName = typeof(TStartup).Assembly.GetName().Name;
            var currentEnviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var consulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");
            var consulKeysValue = Environment.GetEnvironmentVariable(assemblyName + "_CONSUL_KEYS");
            var environmentType = Environment.GetEnvironmentVariable("ENVIRONMENT_TYPE");
            Console.WriteLine($"ENVIRONMENT_TYPE => {environmentType}");
            if (environmentType?.ToUpper() == "IIS")
            {
                builder.UseIIS();
            }
            else if (environmentType?.ToUpper() == "KESTREL")
            {
                builder.UseKestrel();
            }
            builder.ConfigureAppConfiguration((c) =>
            {
                #region Consul Configuration

                var consulEnabled = !string.IsNullOrEmpty(consulAddress);

                if (consulEnabled)
                {

                    if (!string.IsNullOrEmpty(consulKeysValue))
                    {
                        var consulKeys = consulKeysValue.Split(',').ToArray();
                        foreach (var consulKey in consulKeys)
                        {
                            c.AddConsul(
                               $"{consulKey}/{currentEnviroment}", (options) =>
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
                                    $"{assemblyName}/{currentEnviroment}", (options) =>
                                    {
                                        options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulAddress); };
                                        options.Optional = false;
                                        options.ReloadOnChange = true;
                                        options.OnLoadException = exceptionContext => { exceptionContext.Ignore = false; };
                                    });
                    }
                }



                #endregion
            });


            builder.UseSerilog();
        }

    }

}
