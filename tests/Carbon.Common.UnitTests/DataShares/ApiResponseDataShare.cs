using Carbon.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Common.UnitTests.DataShares
{
    public class IsSuccessTrueApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK };
        }
    }
    public class IsSuccessFalseApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, 1 };
        }
    }
    public class SetStatusAndIdentifierApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.BadRequest };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.Conflict };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.InternalServerError };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.NotFound };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.RequestTimeout };
            yield return new object[] { string.Empty, ApiStatusCode.UnAuthorized };
            yield return new object[] { null, ApiStatusCode.UnAuthorized };
        }
    }
    public class SetStatusIdentifierMessagesApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.BadRequest, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.Conflict, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.InternalServerError, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.NotFound, null };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.RequestTimeout, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { string.Empty, ApiStatusCode.UnAuthorized, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { null, ApiStatusCode.UnAuthorized, new List<string>() { "First Message", "Second Message" } };
        }
    }
    public class AddValidMessageToApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, "Test Message", 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, "Test Message", 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.BadRequest, "Test Message", 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.Conflict, "Test Message", 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.InternalServerError, "Test Message", 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.NotFound, "Test Message", 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.RequestTimeout, "Test Message", 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { string.Empty, ApiStatusCode.UnAuthorized, "Test Message", 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { null, ApiStatusCode.UnAuthorized, "Test Message", 1, new List<string>() { "First Message", "Second Message" } };
        }
    }
    public class AddNullMessagesToApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, 1, null };
        }
    }

    public class AddNullMessageToApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, null, 1 };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, null, 1 };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, null, 1 };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.BadRequest, null, 1 };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.Conflict, null, 1 };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.InternalServerError, null, 1 };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.NotFound, null, 1 };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.RequestTimeout, null, 1 };
            yield return new object[] { string.Empty, ApiStatusCode.UnAuthorized, null, 1 };
            yield return new object[] { null, ApiStatusCode.UnAuthorized, null, 1 };
        }
    }
    public class AddValidMessagesToApiResponse : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.BadRequest, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.Conflict, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.InternalServerError, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.NotFound, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.RequestTimeout, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { string.Empty, ApiStatusCode.UnAuthorized, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { null, ApiStatusCode.UnAuthorized, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.BadRequest, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.Conflict, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.InternalServerError, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.NotFound, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.RequestTimeout, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { string.Empty, ApiStatusCode.UnAuthorized, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
            yield return new object[] { null, ApiStatusCode.UnAuthorized, 1, new List<string>() { "First Message", "Second Message" }, new string[] { "Third Message", "Fourth Message" } };
        }
    }
}
