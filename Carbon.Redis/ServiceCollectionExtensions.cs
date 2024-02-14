using Carbon.Redis.Builder;
using Carbon.Redis.Sentinel;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using StackExchange.Redis;

using System;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;

using static StackExchange.Redis.Role;

namespace Carbon.Redis
{
    public static class ServiceCollectionExtensions
    {
        public async static void ValidateRedisConnection(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var redisDatabase = serviceScope.ServiceProvider.GetRequiredService<IDatabase>();
                await redisDatabase.StringSetAsync(RedisConstants.RedisKeyLengthKey, RedisHelper.RedisKeyLength);
            }
        }


        /// <summary>
        /// Use AddRedisPersister for implementation Redis
        /// </summary>
        /// <remarks>
        /// If configuration for <see cref="RedisSettings.Enabled"/> is false, adds a <see cref="DummyConnectionMultiplexer"/>
        /// </remarks>
        /// <param name="configuration"></param>
        /// <returns> IServiceCollection </returns>
        public static ICarbonRedisBuilder AddRedisPersister(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            IOptions<RedisSettings> redisSettings = new RedisSettings();
            configuration.GetSection(RedisConstants.RedisSectionName).Bind(redisSettings);
            services.AddSingleton(redisSettings);
            ConfigurationOptions configurationOptions = new ConfigurationOptions();
            if (redisSettings.Value.Enabled)
            {
                if (redisSettings.Value.EndPoints == null || !redisSettings.Value.EndPoints.Any())
                {
                    throw new ArgumentNullException(nameof(redisSettings.Value.EndPoints));
                }

                EndPointCollection endPoints = new EndPointCollection();
                foreach (var endpoint in redisSettings.Value.EndPoints)
                {
                    endPoints.Add(endpoint);
                }

                configurationOptions = new ConfigurationOptions
                {
                    EndPoints = endPoints,
                    KeepAlive = redisSettings.Value.KeepAlive,
                    AbortOnConnectFail = redisSettings.Value.AbortOnConnectFail,
                    ConfigurationChannel = redisSettings.Value.ConfigurationChannel,
                    TieBreaker = redisSettings.Value.TieBreaker,
                    ConfigCheckSeconds = redisSettings.Value.ConfigCheckSeconds,
                    CommandMap = redisSettings.Value.CommandMap != null && redisSettings.Value.CommandMap.Any() ? CommandMap.Create(redisSettings.Value.CommandMap, available: redisSettings.Value.CommandMapAvailable) : CommandMap.Default,
                    Password = redisSettings.Value.Password,
                    AllowAdmin = redisSettings.Value.AllowAdmin,
                    AsyncTimeout = redisSettings.Value.AsyncTimeout,
                    ConnectRetry = redisSettings.Value.ConnectRetry,
                    ConnectTimeout = redisSettings.Value.ConnectTimeout,
                    DefaultDatabase = redisSettings.Value.DefaultDatabase,
                    Ssl = redisSettings.Value.SSLEnabled,
                    ServiceName = redisSettings.Value.SentinelServiceName,
                    SyncTimeout = redisSettings.Value.SyncTimeout,
                    Protocol = RedisProtocol.Resp2
                    //CheckCertificateRevocation = false
                };
                //configurationOptions.CertificateValidation += ConfigurationOptions_CertificateValidation;

                if (!String.IsNullOrEmpty(redisSettings.Value.User))
                    configurationOptions.User = redisSettings.Value.User;

                if (redisSettings.Value.SSLEnabled)
                {
                    if (redisSettings.Value.SslProtocols.HasValue)
                        configurationOptions.SslProtocols = redisSettings.Value.SslProtocols.Value;
                    else
                        configurationOptions.SslProtocols = System.Security.Authentication.SslProtocols.None;
                }

                services.AddSingleton<ConfigurationOptions>(configurationOptions);
                if (!String.IsNullOrEmpty(configurationOptions.ServiceName))
                {
                    var sentinelConfig = configurationOptions.Clone();
                    var secondsOfTimeOut = 1000;//1 seconds, if the value becomes lower than a second, it can cause errors.
                    sentinelConfig.SyncTimeout = sentinelConfig.SyncTimeout < secondsOfTimeOut ? secondsOfTimeOut : sentinelConfig.SyncTimeout;
                    sentinelConfig.AsyncTimeout = sentinelConfig.AsyncTimeout < secondsOfTimeOut ? secondsOfTimeOut : sentinelConfig.AsyncTimeout;
                    sentinelConfig.CommandMap = CommandMap.Sentinel;
                    SentinelConnectionMultiplexer redisSentinel = new SentinelConnectionMultiplexer(ConnectionMultiplexer.SentinelConnect(sentinelConfig));
                    if (redisSentinel.ConnectionMultiplexer.IsConnected)
                    {
                        var master = redisSentinel.ConnectionMultiplexer.GetSentinelMasterConnection(configurationOptions);
                        services.AddSingleton<IConnectionMultiplexer>(master);
                        var db = master.GetDatabase(redisSettings.Value.DefaultDatabase);
                        services.AddSingleton(s => db);
                        RedisHelper.SetRedisKeyLength(redisSettings.Value.KeyLength);
                        services.AddSingleton<ISentinelConnectionMultiplexer>(redisSentinel);
                    }
                    else
                    {
                        throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, ConnectionFailureType.UnableToConnect.ToString());
                    }
                }
                else
                {
                    var redis = ConnectionMultiplexer.Connect(configurationOptions);
                    if (redis.IsConnected)
                    {
                        services.AddSingleton<IConnectionMultiplexer>(redis);
                        var db = redis.GetDatabase(redisSettings.Value.DefaultDatabase);
                        services.AddSingleton(s => db);
                        RedisHelper.SetRedisKeyLength(redisSettings.Value.KeyLength);
                        NonSentinelConnectionMultiplexer redisSentinel = new NonSentinelConnectionMultiplexer(redis);
                        services.AddSingleton<ISentinelConnectionMultiplexer>(redisSentinel);

                    }
                    else
                    {
                        throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, ConnectionFailureType.UnableToConnect.ToString());
                    }
                }
                services.AddHealthChecks().AddCheck<CustomRedisHealthCheck>("RedisConnectionCheck", HealthStatus.Unhealthy);
                
            }
            else
            {
                services.AddSingleton<IDatabase, RedisDatabase>();
                services.AddSingleton<IConnectionMultiplexer, DummyConnectionMultiplexer>();
            }

            
            return new CarbonRedisBuilder(services, configurationOptions, configuration);
        }

        private static bool ConfigurationOptions_CertificateValidation(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            return false;
        }

        /// <summary>
        /// Converting password to MD5 for security
        /// </summary>
        /// <param name="password">password</param>
        /// <returns> MD5 string </returns>
        public static string ConvertToMD5(string password)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            bytes = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();

            foreach (byte ba in bytes)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }
    }
}