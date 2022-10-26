using System;
using System.Collections.Generic;
using System.Text;
using Carbon.Redis.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.Redis.UnitTests.StaticWrappers.ServiceCollectionExtensions
{
    public interface IServiceCollectionExtensionsWrapper
    {
       public ICarbonRedisBuilder AddRedisPersister(IServiceCollection services, IConfiguration configuration);
       public string ConvertToMD5(string password);
    }
}
