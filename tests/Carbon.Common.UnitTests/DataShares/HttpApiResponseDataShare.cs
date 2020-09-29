using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Common.UnitTests.DataShares
{
    public class IdentifierHttpStatusCodeHttpApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.OK };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.Created };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.NoContent };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.BadRequest };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.UpgradeRequired };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.UnsupportedMediaType };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.RequestUriTooLong };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.RequestEntityTooLarge };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.RequestedRangeNotSatisfiable };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.ProxyAuthenticationRequired };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.PaymentRequired };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.NotAcceptable };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.MethodNotAllowed };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.Gone };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.ExpectationFailed };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.Conflict };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.GatewayTimeout };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.ServiceUnavailable };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.HttpVersionNotSupported };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.NotImplemented };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.BadGateway };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.InternalServerError };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.LoopDetected };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.InsufficientStorage };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.VariantAlsoNegotiates };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.NotFound };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.RequestTimeout };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.Unauthorized };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.Forbidden };
        }
    }
    public class IdentifierHttpStatusCodeErrorCodeHttpApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.OK, 1 };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.Created, 1 };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.NoContent, 2 };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.BadRequest, 6 };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.UpgradeRequired, 3 };

        }
    }
    public class IdentifierHttpStatusCodeMessagesHttpApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.OK, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.Created, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.NoContent, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.BadRequest, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.UpgradeRequired, null };

        }
    }
    public class IdentifierHttpStatusCodeErrorCodeMessagesHttpApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.OK, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.Created, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.NoContent, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.BadRequest, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), HttpStatusCode.UpgradeRequired, 1, null };

        }
    }

}
