using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.WebApplication.UnitTests.DataShares
{
    public class IServiceCollectionExtensionsDataShare : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString() };
        }
    }
}
