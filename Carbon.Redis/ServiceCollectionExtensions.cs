using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Carbon.Redis
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisPersister(this IServiceCollection services, IConfiguration configuration)
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
            configuration.GetSection("Redis").Bind(redisSettings);
            services.AddSingleton(redisSettings);
            if (redisSettings.Value.Enabled)
            {


                var configurationOptions = new ConfigurationOptions
                {
                    EndPoints = { string.Join(",", redisSettings.Value.EndPoints) },
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

                };
                try
                {

                    var redis = ConnectionMultiplexer.Connect(configurationOptions);

                    if (redis.IsConnected)
                    {
                        services.AddSingleton<IConnectionMultiplexer>(redis);
                        var db = redis.GetDatabase(redisSettings.Value.DefaultDatabase);
                        services.AddSingleton(s => db);
                        _ = db.StringSet("redisKeyLength", redisSettings.Value.KeyLength);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(configurationOptions.Password))
                        {
                            configurationOptions.Password = ConvertToMD5(configurationOptions.Password);
                        }
                        throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, Newtonsoft.Json.JsonConvert.SerializeObject(configurationOptions));
                    }
                }
                catch (RedisException ex)
                {

                    throw ex;
                }
            }


            return services;
        }
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



