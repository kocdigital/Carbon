using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.Redis.UnitTests.StaticWrappers.ServiceCollectionExtensions
{
    public class ServiceCollectionExtensionsWrapper : IServiceCollectionExtensionsWrapper
    {
        public IServiceCollection AddRedisPersister(IServiceCollection services, IConfiguration configuration)
        {
          return Redis.ServiceCollectionExtensions.AddRedisPersister(services, configuration);
        }

        public string ConvertToMD5(string password)
        {
            return Redis.ServiceCollectionExtensions.ConvertToMD5(password);
        }
    }
}
