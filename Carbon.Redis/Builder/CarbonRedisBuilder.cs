using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Redis.Builder
{
    public class CarbonRedisBuilder : ICarbonRedisBuilder
    {
        public CarbonRedisBuilder(IServiceCollection services, ConfigurationOptions configurationOptions, IConfiguration configuration)
        {
            Services = services;
            ConfigurationOptions = configurationOptions;
            Configuration = configuration;
        }
        public IServiceCollection Services { get; set; }

        public ConfigurationOptions ConfigurationOptions { get; set; }
        public IConfiguration Configuration { get; set; }
    }
}
