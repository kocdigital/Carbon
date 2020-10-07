using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.HttpClient.UnitTests.StaticWrappers
{
    public interface IServiceCollectionExtensionsWrapper
    {
        void AddHttpClientWithHeaderPropagation(IServiceCollection services, Action<HeaderPropagationOptions> configureHeaderPropagation);
    }
}
