using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace Carbon.MassTransit.UnitTests.DataShares
{
    //IServiceCollection services, Action<IServiceCollectionConfigurator> configurator)
    public class AddMassTransitBusToServiceCollection : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString() };
        }
    }
}
