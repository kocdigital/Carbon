using Carbon.WebApplication;
using Carbon.WebApplication.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.WebApplication.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper
{
    public class ServiceCollectionExtensionsWrapper : IServiceCollectionExtensionsWrapper
    {
        public IServiceCollection AddBearerAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            return ServiceCollectionExtensions.AddBearerAuthentication(services, configuration);
        }
    }
}
