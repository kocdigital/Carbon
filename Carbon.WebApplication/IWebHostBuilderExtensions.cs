using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
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
        /// <param name="builder"></param>
        public static void UseCarbonFeatures(this IWebHostBuilder builder)
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
            var currentEnviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var consulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");
            var consulKeysValue = Environment.GetEnvironmentVariable(assemblyName + "_CONSUL_KEYS");
            var environmentType = Environment.GetEnvironmentVariable("ENVIRONMENT_TYPE");

            var confType = Environment.GetEnvironmentVariable("CONFIGURATION_TYPE");
            Console.WriteLine("Configuration Type: " + confType);

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
                #region Configuration

                //Suitable for Standalone or IIS Applications
                if (String.IsNullOrEmpty(confType) || confType?.ToUpper() == "CONSUL")
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
                }
                //Suitable for Kubernetes or Dockerized Applications
                else if (confType?.ToUpper() == "FILE" || confType == "file" || confType == "File")
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
                    Console.WriteLine("No Configuration Source Specified!");
                }


                #endregion
            });
            builder.UseSerilog();
        }

    }

}
