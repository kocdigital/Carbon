using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.Redis.UnitTests.StaticWrappers.ServiceCollectionExtensions
{
    public interface IServiceCollectionExtensionsWrapper
    {
       public IServiceCollection AddRedisPersister(IServiceCollection services, IConfiguration configuration);
       public string ConvertToMD5(string password);
    }
}
