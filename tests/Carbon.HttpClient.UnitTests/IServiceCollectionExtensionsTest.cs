using Carbon.Common.UnitTests.DataShares;
using Carbon.HttpClient.UnitTests.DataShares;
using Carbon.HttpClient.UnitTests.StaticWrappers;
using Carbon.HttpClients;
using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.HttpClient.UnitTests
{
    public class IServiceCollectionExtensionsTest
    {
        [Theory]
        [AddHttpClientWithHeaderPropagationData]
        public void AddHttpClientWithHeaderPropagation_Successfully_void(IServiceCollection services, Action<HeaderPropagationOptions> configureHeaderPropagation)
        {
            // Arrange
            var wrapper = new IIServiceCollectionExtensionsWrapper();

            // Act
            wrapper.AddHttpClientWithHeaderPropagation(services, configureHeaderPropagation);

            // Assert      
            Assert.IsType<IIServiceCollectionExtensionsWrapper>(wrapper);

        }
    }
}
