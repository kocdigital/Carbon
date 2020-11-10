using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.WebApplication.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper
{
    public interface IServiceCollectionExtensionsWrapper
    {
        IServiceCollection AddBearerAuthentication(IServiceCollection services, IConfiguration configuration);
    }
}
