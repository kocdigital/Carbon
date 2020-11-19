using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.WebApplication.UnitTests.DataShares
{
    public class InternalServerErrorObjectResultData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { "sample" };
            yield return new object[] { null };
        }
    }
}
