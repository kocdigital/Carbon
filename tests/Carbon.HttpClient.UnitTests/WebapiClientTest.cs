using Carbon.Common.UnitTests.DataShares;
using Carbon.HttpClients;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.HttpClient.UnitTests
{
    public class WebapiClientFacts : WebapiClientTest<List<string>> { public WebapiClientFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }

    public abstract class WebapiClientTest<T>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        protected WebapiClientTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [HttpClientRequest]
        public void WebapiClient_Successfully_Null(System.Net.Http.HttpClient client)
        {
            // Arrange

            // Act
            WebapiClient response = new WebapiClient(client);

            // Assert
            Assert.IsType<WebapiClient>(response);

            _testOutputHelper.WriteLine("Test passed!");
        }

    }
}
