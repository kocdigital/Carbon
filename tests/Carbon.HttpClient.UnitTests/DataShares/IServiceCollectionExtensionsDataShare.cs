using Carbon.Test.Common.DataShares;
using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace Carbon.HttpClient.UnitTests.DataShares
{
    public class AddHttpClientWithHeaderPropagationData : DataAttribute
    {
        //IServiceCollection services, Action<HeaderPropagationOptions> configureHeaderPropagation
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            yield return new object[] { serviceCollection, null };
        }
    }
}
