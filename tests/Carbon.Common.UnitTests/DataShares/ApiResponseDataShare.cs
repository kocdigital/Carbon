using Carbon.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Common.UnitTests.DataShares
{
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
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, null, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, null, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.OK, null, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.BadRequest, null, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.Conflict, null, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.InternalServerError, null, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.NotFound, null, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { Guid.NewGuid().ToString(), ApiStatusCode.RequestTimeout, null, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { string.Empty, ApiStatusCode.UnAuthorized, null, 1, new List<string>() { "First Message", "Second Message" } };
            yield return new object[] { null, ApiStatusCode.UnAuthorized, null, 1, new List<string>() { "First Message", "Second Message" } };
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
