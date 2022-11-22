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
using System.Security.Cryptography;
using System.Text;

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
                if(redisSettings.Value.EndPoints == null || !redisSettings.Value.EndPoints.Any())
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
                    CommandMap = CommandMap.Create(redisSettings.Value.CommandMap, available: redisSettings.Value.CommandMapAvailable),
                    Password = redisSettings.Value.Password,
                    AllowAdmin = redisSettings.Value.AllowAdmin,
                    AsyncTimeout = redisSettings.Value.AsyncTimeout,
                    ConnectRetry = redisSettings.Value.ConnectRetry,
                    ConnectTimeout = redisSettings.Value.ConnectTimeout,
                    DefaultDatabase = redisSettings.Value.DefaultDatabase,
                    Ssl = redisSettings.Value.SSLEnabled,
                    ServiceName = redisSettings.Value.SentinelServiceName,
                    SyncTimeout = redisSettings.Value.SyncTimeout
                };

                if (redisSettings.Value.SSLEnabled)
                {
                    configurationOptions.SslProtocols = System.Security.Authentication.SslProtocols.None;
                }

                try
                {
                    var redis = ConnectionMultiplexer.Connect(configurationOptions);
                    if (redis.IsConnected)
                    {
                        services.AddSingleton<ConfigurationOptions>(configurationOptions);
                        services.AddSingleton<IConnectionMultiplexer>(redis);
                        var db = redis.GetDatabase(redisSettings.Value.DefaultDatabase);
                        services.AddSingleton(s => db);
                        RedisHelper.SetRedisKeyLength(redisSettings.Value.KeyLength);

                        if (!String.IsNullOrEmpty(configurationOptions.ServiceName))
                        {
                            var sentinelConfig = configurationOptions.Clone();
                            var SecondsOfTimeOut = 1000;
                            sentinelConfig.SyncTimeout = SecondsOfTimeOut;
                            sentinelConfig.AsyncTimeout = SecondsOfTimeOut;
                            SentinelConnectionMultiplexer redisSentinel = new SentinelConnectionMultiplexer(ConnectionMultiplexer.SentinelConnect(sentinelConfig));
                            if (redisSentinel.ConnectionMultiplexer.IsConnected)
                            {
                                services.AddSingleton<ISentinelConnectionMultiplexer>(redisSentinel);
                            }
                        }
                        else
                        {
                            NonSentinelConnectionMultiplexer redisSentinel = new NonSentinelConnectionMultiplexer(redis);
                            services.AddSingleton<ISentinelConnectionMultiplexer>(redisSentinel);
                        }

                        services.AddHealthChecks().AddCheck<CustomRedisHealthCheck>("RedisConnectionCheck", HealthStatus.Unhealthy);
                    }
                    else
                    {
                        throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, ConnectionFailureType.UnableToConnect.ToString());
                    }
                }
                catch (RedisException ex)
                {
                    throw ex;
                }
            }
            else
            {
                services.AddSingleton<IDatabase, RedisDatabase>();
                services.AddSingleton<IConnectionMultiplexer, DummyConnectionMultiplexer>();
            }


            return new CarbonRedisBuilder(services, configurationOptions, configuration);
        }

        /// <summary>
        /// Converting password to MD5 for security
        /// </summary>
        /// <param name="password">password</param>
        /// <returns> MD5 string </returns>
        public static string ConvertToMD5(string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
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



