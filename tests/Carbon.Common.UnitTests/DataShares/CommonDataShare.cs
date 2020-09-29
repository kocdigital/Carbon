using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Common.UnitTests.DataShares
{
    public class ValidApiStatusCode : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { ApiStatusCode.OK };
            yield return new object[] { ApiStatusCode.BadRequest };
            yield return new object[] { ApiStatusCode.Conflict };
            yield return new object[] { ApiStatusCode.InternalServerError };
            yield return new object[] { ApiStatusCode.NotFound };
            yield return new object[] { ApiStatusCode.RequestTimeout };
            yield return new object[] { ApiStatusCode.UnAuthorized };

        }
    }
}
