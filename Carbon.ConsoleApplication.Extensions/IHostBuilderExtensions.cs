using Carbon.Common;
using Carbon.Serilog;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Enrichers.Sensitive;

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using Carbon.ConsulSecretEncryptor.Core;
using Microsoft.Extensions.Primitives;

using Winton.Extensions.Configuration.Consul;

namespace Carbon.ConsoleApplication
{
    public static class IHostBuilderExtensions
    {
        /// <summary>
        /// Configures given <see cref="IHostBuilder"/> with given configurations
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/></typeparam>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureApp">Injects action for user defined configurations</param>
        /// <param name="configureServices">Injects action for user defined service configurations. <see cref="IHostBuilder.ConfigureServices"/></param>
        /// <returns>Configured <paramref name="builder"/></returns>
        private static IHostBuilder UseFeatures<TProgram>(IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureApp, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            var assemblyName = typeof(TProgram).Assembly.GetName().Name;
            var currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopmentEnvironment = string.Equals(currentEnvironment, "Development", StringComparison.OrdinalIgnoreCase);
            var consulAddress = Environment.GetEnvironmentVariable("CONSUL_ADDRESS");
            var consulKeysValue = Environment.GetEnvironmentVariable(assemblyName + "_CONSUL_KEYS");
            var confType = Environment.GetEnvironmentVariable("CONFIGURATION_TYPE");

            builder.ConfigureAppConfiguration((h, c) =>
            {
                c.AddEnvironmentVariables();

                #region Consul Configuration
                if (string.IsNullOrEmpty(confType) || string.Equals(confType, "CONSUL", StringComparison.InvariantCultureIgnoreCase))
                {
                    var consulEnabled = !string.IsNullOrEmpty(consulAddress);

                    if (consulEnabled)
                    {
                        Console.WriteLine("Configuration Type: CONSUL");
                        var consulAclToken = ResolveConsulAclToken();
                        var consulBasicAuthCredential = ResolveConsulBasicAuthCredential();
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
                                   $"{consulKey}/{currentEnvironment}", (options) =>
                                   {
                                       options.ConsulConfigurationOptions = cco =>
                                       {
                                           cco.Address = new Uri(consulAddress);
                                           if (!string.IsNullOrEmpty(consulAclToken))
                                               cco.Token = consulAclToken;
                                           if (consulBasicAuthCredential != null)
                                           {
                                               // Winton's wrapper does not expose the HttpHandler override Consul.NET recommends instead of HttpAuth.
#pragma warning disable CS0618
                                               cco.HttpAuth = consulBasicAuthCredential;
#pragma warning restore CS0618
                                           }
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
                                        $"{assemblyName}/{currentEnvironment}", (options) =>
                                        {
                                            options.ConsulConfigurationOptions = cco =>
                                            {
                                                cco.Address = new Uri(consulAddress);
                                                if (!string.IsNullOrEmpty(consulAclToken))
                                                    cco.Token = consulAclToken;
                                                if (consulBasicAuthCredential != null)
                                                {
                                                    // Winton's wrapper does not expose the HttpHandler override Consul.NET recommends instead of HttpAuth.
#pragma warning disable CS0618
                                                    cco.HttpAuth = consulBasicAuthCredential;
#pragma warning restore CS0618
                                                }
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
                else if (string.Equals(confType, "FILE", StringComparison.InvariantCultureIgnoreCase))
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
                            if (isDevelopmentEnvironment)
                            {
                                Console.WriteLine("Inserting Config => \n" + configToRead);
                            }
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
                    Console.WriteLine($"Unknown Configuration Source Specified! Defaults to => appsettings.{currentEnvironment}.json");
                    c.AddJsonFile($"appsettings.{currentEnvironment}.json", optional: true);
                }
                #endregion

                var configuration = c.Build();

                Log.Logger = SerilogExtensions.CreateLogger(configuration);

                configureApp?.Invoke(h, c);

            }).ConfigureServices((h, s) =>
            {
                void DecryptConsulConfigValues() => ConsulConfigValueProtector.DecryptInPlace(h.Configuration, BuildConsulSecretCertificateSelection);
                DecryptConsulConfigValues();
                ChangeToken.OnChange(() => h.Configuration.GetReloadToken(), DecryptConsulConfigValues);

                configureServices?.Invoke(h, s);
            });

            
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            return builder.UseSerilog(); 
        }

        /// <summary>
        /// Event handler for <see cref="AppDomain.UnhandledException"/>. Adds Log as Error
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="e"><see cref="UnhandledExceptionEventArgs"/></param>
        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                var ex = e.ExceptionObject as Exception;
                Log.Logger.Error("Unhandled exception occured! {0}: {1}\n {2}", e.ExceptionObject.GetType(), ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Use for configuring given <see cref="IHostBuilder"/> with Carbon Standards. 
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/></typeparam>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureServices">Injects action for user defined service configurations. <see cref="IHostBuilder.ConfigureServices"/></param>
        /// <returns>Configured <paramref name="builder"/></returns>
        /// <remarks>Carbon adds Consul and Serilog support.</remarks>
        public static IHostBuilder AddCarbonServices<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class => UseFeatures<TProgram>(builder, null, configureServices);

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

            var selection = BuildConsulSecretCertificateSelection();
            Console.WriteLine($"Consul ACL Token: ENCRYPTED (decrypting via certificate)");
            using var certificate = CertificateResolver.Resolve(selection, requirePrivateKey: true);
            using var rsa = certificate.GetRSAPrivateKey()
                ?? throw new InvalidOperationException($"Certificate '{certificate.Thumbprint}' does not expose an accessible RSA private key.");
            return ConsulSecretCrypto.DecryptPlain(encryptedToken, rsa);
        }

        /// <summary>
        /// Resolves HTTP Basic Auth credentials used when Consul sits behind a reverse proxy (e.g. nginx) that
        /// enforces Basic Auth in front of it. Neither the username nor the password is ever placed in the
        /// environment as plaintext: CONSUL_BASIC_AUTH_ENCRYPTED must hold an RSA-OAEP/SHA256 (base64) ciphertext
        /// that decrypts to a single "username:password" value, and is only decrypted here using the private key
        /// of a certificate installed on this machine.
        /// </summary>
        private static NetworkCredential ResolveConsulBasicAuthCredential()
        {
            var encryptedCredential = Environment.GetEnvironmentVariable("CONSUL_BASIC_AUTH_ENCRYPTED");
            if (string.IsNullOrWhiteSpace(encryptedCredential))
                return null;

            var selection = BuildConsulSecretCertificateSelection();
            Console.WriteLine($"Consul Basic Auth: ENABLED (credential decrypted via certificate)");
            using var certificate = CertificateResolver.Resolve(selection, requirePrivateKey: true);
            using var rsa = certificate.GetRSAPrivateKey()
                ?? throw new InvalidOperationException($"Certificate '{certificate.Thumbprint}' does not expose an accessible RSA private key.");
            var decryptedCredential = ConsulSecretCrypto.DecryptPlain(encryptedCredential, rsa);
            var (username, password) = ConsulSecretCrypto.SplitBasicAuthCredential(decryptedCredential);
            return new NetworkCredential(username, password);
        }

        /// <summary>
        /// Builds the certificate selection (by thumbprint or subject, in the given store) used to decrypt Consul
        /// secrets. Shared by the ACL token, Basic Auth, and config-value resolution so all rely on the same
        /// machine-installed certificate.
        /// </summary>
        private static CertificateSelection BuildConsulSecretCertificateSelection()
        {
            var thumbprint = Environment.GetEnvironmentVariable("CONSUL_ACL_CERT_THUMBPRINT");
            var subjectName = Environment.GetEnvironmentVariable("CONSUL_ACL_CERT_SUBJECT");
            if (string.IsNullOrWhiteSpace(thumbprint) && string.IsNullOrWhiteSpace(subjectName))
                throw new InvalidOperationException("Neither CONSUL_ACL_CERT_THUMBPRINT nor CONSUL_ACL_CERT_SUBJECT is set. A certificate installed on this machine is required to decrypt Consul secrets.");

            var storeLocation = Enum.TryParse<StoreLocation>(Environment.GetEnvironmentVariable("CONSUL_ACL_CERT_STORE_LOCATION"), true, out var parsedLocation)
                ? parsedLocation
                : StoreLocation.CurrentUser;
            var storeName = Enum.TryParse<StoreName>(Environment.GetEnvironmentVariable("CONSUL_ACL_CERT_STORE_NAME"), true, out var parsedName)
                ? parsedName
                : StoreName.My;

            return new CertificateSelection { Thumbprint = thumbprint, SubjectName = subjectName, StoreLocation = storeLocation, StoreName = storeName };
        }

        /// <summary>
        /// Configures <paramref name="builder"/> with given <paramref name="configureServiceProviders"/>
        /// </summary>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureServiceProviders"><see cref="Action"/> for configuring services. <seealso cref="IHostBuilder.ConfigureServices(Action{HostBuilderContext, IServiceCollection})"/></param>
        /// <returns>Given <paramref name="builder"/></returns>
        public static IHostBuilder UseServiceProvider(this IHostBuilder builder, Action<IServiceProvider> configureServiceProviders)
        {
            builder.ConfigureServices((h, s) =>
            {
                var serviceProvider = s.BuildServiceProvider();
                configureServiceProviders(serviceProvider);
            });

            return builder;
        }
        /// <summary>
        /// Use for configuring given <see cref="IHostBuilder"/> with Carbon Standards. 
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/></typeparam>
        /// <param name="builder"><see cref="IHostBuilder"/> to configure</param>
        /// <param name="configureApp">Injects action for user defined configurations.</param>
        /// <returns>Configured <paramref name="builder"/></returns>
        /// <remarks>Carbon adds Consul and Serilog support.</remarks>
        public static IHostBuilder AddCarbonConfiguration<TProgram>(this IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureApp) where TProgram : class => UseFeatures<TProgram>(builder, configureApp, null);
    }
}
