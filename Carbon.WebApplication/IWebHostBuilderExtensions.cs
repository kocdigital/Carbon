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
using System.Security.Cryptography.X509Certificates;
using Winton.Extensions.Configuration.Consul;

namespace Carbon.WebApplication
{

    public static class IWebHostBuilderExtensions
    {
        /// <summary>
        /// Applies Carbon Framework settings such as Environment, Consul Address, Assembly Name etc.
        /// </summary>
        /// <param name="builder"></param>
        public static void UseCarbonFeatures(this IWebHostBuilder builder, bool useLocalConfiguration = false)
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

            if (useLocalConfiguration)
            {
#if DEBUG
                Console.WriteLine("External configuration sources are disabled.");
#endif
            }
            else
            {
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
                            var consulAclToken = ResolveConsulAclToken();
                            var consulReloadOnChange = !bool.TryParse(Environment.GetEnvironmentVariable("CONSUL_RELOAD_ON_CHANGE"), out var parsedReloadOnChange) || parsedReloadOnChange;
                            var consulPollWaitTime = int.TryParse(Environment.GetEnvironmentVariable("CONSUL_POLL_WAIT_TIME_SECONDS"), out var parsedPollWaitTimeSeconds)
                                ? TimeSpan.FromSeconds(parsedPollWaitTimeSeconds)
                                : (TimeSpan?)null;
                            if (!consulReloadOnChange)
                                Console.WriteLine("Consul Reload On Change: DISABLED (no long-poll watch traffic will be sent)");
                            else if (consulPollWaitTime.HasValue)
                                Console.WriteLine("Consul Poll Wait Time: " + consulPollWaitTime.Value);
                            if (!string.IsNullOrEmpty(consulKeysValue))
                            {
                                var consulKeys = consulKeysValue.Split(',').ToArray();
                                foreach (var consulKey in consulKeys)
                                {
                                    c.AddConsul(
                                       $"{consulKey}/{currentEnviroment}", (options) =>
                                       {
                                           options.ConsulConfigurationOptions = cco =>
                                           {
                                               cco.Address = new Uri(consulAddress);
                                               if (!string.IsNullOrEmpty(consulAclToken))
                                                   cco.Token = consulAclToken;
                                           };
                                           options.Optional = false;
                                           options.ReloadOnChange = consulReloadOnChange;
                                           if (consulPollWaitTime.HasValue)
                                               options.PollWaitTime = consulPollWaitTime.Value;
                                           options.OnLoadException = exceptionContext => { exceptionContext.Ignore = false; };
                                       });
                                }
                            }
                            else
                            {
                                c.AddConsul(
                                            $"{assemblyName}/{currentEnviroment}", (options) =>
                                            {
                                                options.ConsulConfigurationOptions = cco =>
                                                {
                                                    cco.Address = new Uri(consulAddress);
                                                    if (!string.IsNullOrEmpty(consulAclToken))
                                                        cco.Token = consulAclToken;
                                                };
                                                options.Optional = false;
                                                options.ReloadOnChange = consulReloadOnChange;
                                                if (consulPollWaitTime.HasValue)
                                                    options.PollWaitTime = consulPollWaitTime.Value;
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
            }

#if NET6_0
            builder.UseSerilog();
#endif
        }

        /// <summary>
        /// Resolves the Consul ACL token from an encrypted environment variable. The plaintext token is never
        /// placed in the environment: it must be pre-encrypted (RSA-OAEP/SHA256) against the public key of a
        /// certificate installed on this machine, and is only decrypted here using that certificate's private key.
        /// </summary>
        private static string ResolveConsulAclToken()
        {
            var encryptedToken = Environment.GetEnvironmentVariable("CONSUL_ACL_TOKEN_ENCRYPTED");
            if (string.IsNullOrWhiteSpace(encryptedToken))
                return null;

            var thumbprint = Environment.GetEnvironmentVariable("CONSUL_ACL_CERT_THUMBPRINT");
            var subjectName = Environment.GetEnvironmentVariable("CONSUL_ACL_CERT_SUBJECT");
            if (string.IsNullOrWhiteSpace(thumbprint) && string.IsNullOrWhiteSpace(subjectName))
                throw new InvalidOperationException("CONSUL_ACL_TOKEN_ENCRYPTED is set but neither CONSUL_ACL_CERT_THUMBPRINT nor CONSUL_ACL_CERT_SUBJECT is set. A certificate installed on this machine is required to decrypt the Consul ACL token.");

            var storeLocation = Enum.TryParse<StoreLocation>(Environment.GetEnvironmentVariable("CONSUL_ACL_CERT_STORE_LOCATION"), true, out var parsedLocation)
                ? parsedLocation
                : StoreLocation.CurrentUser;
            var storeName = Enum.TryParse<StoreName>(Environment.GetEnvironmentVariable("CONSUL_ACL_CERT_STORE_NAME"), true, out var parsedName)
                ? parsedName
                : StoreName.My;

            Console.WriteLine($"Consul ACL Token: ENCRYPTED (decrypting via certificate '{thumbprint ?? subjectName}' from {storeLocation}/{storeName})");
            return ConsulAclTokenProtector.Decrypt(encryptedToken, thumbprint, subjectName, storeName, storeLocation);
        }

    }

}
