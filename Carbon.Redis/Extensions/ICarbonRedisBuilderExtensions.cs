using Carbon.Caching.Abstractions;
using Carbon.Redis;
using Carbon.Redis.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Carbon.Caching.Redis
{
    public static class ICarbonRedisBuilderExtensions
    {
        /// <summary>
        /// Enables you to use CarbonRedisCache in your services to access more extended methods and capabilities (i.e. RedLock, Redis PubSub etc.) and make the legacy apis compatible with this package
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ICarbonRedisBuilder AddCarbonRedisCachingHelper(this ICarbonRedisBuilder builder)
        {
            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.Configuration;
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddOptions();
            IOptions<CarbonRedisCacheOptions> redisSettings = new CarbonRedisCacheOptions();
            string instanceName = configuration.GetSection(RedisConstants.RedisSectionName).GetValue<string>(RedisConstants.InstanceName);

            if(String.IsNullOrEmpty(instanceName))
            {
                throw new ArgumentNullException(instanceName);
            }

            redisSettings.Value.InstanceName = instanceName;
            redisSettings.Value.ConfigurationOptions = builder.ConfigurationOptions;
            redisSettings.Value.Configuration = builder.ConfigurationOptions.ToString();

            services.AddSingleton<ICarbonCache, CarbonRedisCache>();

            services.AddSingleton<IDistributedCache, CarbonRedisCache>();
            services.AddSingleton<ICarbonRedisCache, CarbonRedisCache>();
            services.AddSingleton(redisSettings);


            return new CarbonRedisBuilder(services, builder.ConfigurationOptions, builder.Configuration);
        }
    }
}
