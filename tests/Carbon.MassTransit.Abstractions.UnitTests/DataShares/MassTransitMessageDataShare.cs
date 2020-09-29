using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.MassTransit.Abstractions.UnitTests.DataShares
{
    public class MassTransitMessageContentConstractor : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { string.Empty };
            yield return new object[] { "Test" };
            yield return new object[] { null };
        }
    }
}
