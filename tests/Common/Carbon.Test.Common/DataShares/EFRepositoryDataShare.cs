using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Test.Common.DataShares
{

    public class FoundEntityGetByIdAsyncEFRepository : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.Parse("06956e56-1f8b-4fd9-b4b4-f083c23bb98f")};
        }
    }
    public class NotFoundEntityGetByIdAsyncEFRepository : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid() };
        }
    }
}
