using Carbon.Common;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Common.UnitTests.DataShares
{
    public class ValidApiStatusCodeToApiStatusCodeToHttpStatusMapper : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { HttpStatusCode.OK };
            yield return new object[] { HttpStatusCode.Created };
            yield return new object[] { HttpStatusCode.NoContent };
            yield return new object[] { HttpStatusCode.BadRequest };
            yield return new object[] { HttpStatusCode.UpgradeRequired };
            yield return new object[] { HttpStatusCode.UnsupportedMediaType };
            yield return new object[] { HttpStatusCode.RequestUriTooLong };
            yield return new object[] { HttpStatusCode.RequestEntityTooLarge };
            yield return new object[] { HttpStatusCode.RequestedRangeNotSatisfiable };
            yield return new object[] { HttpStatusCode.ProxyAuthenticationRequired };
            yield return new object[] { HttpStatusCode.PaymentRequired };
            yield return new object[] { HttpStatusCode.NotAcceptable };
            yield return new object[] { HttpStatusCode.MethodNotAllowed };
            yield return new object[] { HttpStatusCode.Gone };
            yield return new object[] { HttpStatusCode.ExpectationFailed };
            yield return new object[] { HttpStatusCode.Conflict };
            yield return new object[] { HttpStatusCode.GatewayTimeout };
            yield return new object[] { HttpStatusCode.ServiceUnavailable };
            yield return new object[] { HttpStatusCode.HttpVersionNotSupported };
            yield return new object[] { HttpStatusCode.NotImplemented };
            yield return new object[] { HttpStatusCode.BadGateway };
            yield return new object[] { HttpStatusCode.InternalServerError };
            yield return new object[] { HttpStatusCode.LoopDetected };
            yield return new object[] { HttpStatusCode.InsufficientStorage };
            yield return new object[] { HttpStatusCode.VariantAlsoNegotiates };
            yield return new object[] { HttpStatusCode.NotFound };
            yield return new object[] { HttpStatusCode.RequestTimeout };
            yield return new object[] { HttpStatusCode.Unauthorized };
            yield return new object[] { HttpStatusCode.Forbidden };
        }
    }
    public class InValidApiStatusCodeToApiStatusCodeToHttpStatusMapper : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { HttpStatusCode.IMUsed };
            yield return new object[] { HttpStatusCode.EarlyHints };
            yield return new object[] { HttpStatusCode.NetworkAuthenticationRequired };

        }
    }


    public class ValidHttpStatusCodeToApiStatusCodeToHttpStatusMapper : DataAttribute
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
    public class InValidHttpStatusCodeToApiStatusCodeToHttpStatusMapper : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { null };
        }
    }
}
