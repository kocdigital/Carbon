using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Xunit.Sdk;

namespace Carbon.MassTransit.UnitTests.DataShares
{
    public class CancellationTokenData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { new CancellationToken() };
            yield return new object[] { new CancellationToken(true) };
            yield return new object[] { new CancellationToken(false) };
        }
    }
}
