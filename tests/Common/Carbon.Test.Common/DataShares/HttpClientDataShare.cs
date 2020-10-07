using Carbon.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Common.UnitTests.DataShares
{
    public class HttpClientRequest : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var client = new HttpClient();
            yield return new object[] { client };
        }
    }
   
    
}
