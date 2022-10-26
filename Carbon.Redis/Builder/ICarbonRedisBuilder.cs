using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Redis.Builder
{
    public interface ICarbonRedisBuilder
    {
        //
        // Summary:
        //     Gets the builder service collection.
        IServiceCollection Services { get; set; }
        ConfigurationOptions ConfigurationOptions { get; set; }
        IConfiguration Configuration { get; set; }
    }

}
