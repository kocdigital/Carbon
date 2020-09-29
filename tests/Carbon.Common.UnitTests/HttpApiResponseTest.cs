using Carbon.Common.UnitTests.DataShares;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.Common.UnitTests
{
    public class DecimalHttpApiResponseTest : HttpApiResponseTest<decimal> { public DecimalHttpApiResponseTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public class StringHttpApiResponseTest : HttpApiResponseTest<string> { public StringHttpApiResponseTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public class DateTimeHttpApiResponseTest : HttpApiResponseTest<DateTime> { public DateTimeHttpApiResponseTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public class Int32HttpApiResponseTest : HttpApiResponseTest<int> { public Int32HttpApiResponseTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public abstract class HttpApiResponseTest<T>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        protected HttpApiResponseTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        [Theory]
        [IdentifierHttpStatusCodeHttpApiResponse]
        public void IdentifierHttpStatusCode_Successfully_HttpApiResponse(string identifier, HttpStatusCode statusCode)
        {
            // Arrange
            HttpApiResponse<T> response = new HttpApiResponse<T>(identifier, statusCode);

            // Act
            var apiStatusCode = statusCode.GetApiStatusCode();
            // Assert

            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(apiStatusCode, response.StatusCode);

            _testOutputHelper.WriteLine("Test passed!");
        }
        [Theory]
        [IdentifierHttpStatusCodeErrorCodeHttpApiResponse]
        public void IdentifierHttpStatusCodeErrorCode_Successfully_HttpApiResponse(string identifier, HttpStatusCode statusCode, int errorCode)
        {
            // Arrange
            HttpApiResponse<T> response = new HttpApiResponse<T>(identifier, statusCode, errorCode);

            // Act
            var apiStatusCode = statusCode.GetApiStatusCode();
            // Assert

            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(apiStatusCode, response.StatusCode);
            Assert.Equal(errorCode, response.ErrorCode);

            _testOutputHelper.WriteLine("Test passed!");
        }
        [Theory]
        [IdentifierHttpStatusCodeMessagesHttpApiResponse]
        public void IdentifierHttpStatusCodeMessages_Successfully_HttpApiResponse(string identifier, HttpStatusCode statusCode, List<string> messages)
        {
            // Arrange
            HttpApiResponse<T> response = new HttpApiResponse<T>(identifier, statusCode, messages);

            // Act
            var apiStatusCode = statusCode.GetApiStatusCode();
            // Assert

            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(apiStatusCode, response.StatusCode);
            Assert.Equal(messages, response.Messages);

            _testOutputHelper.WriteLine("Test passed!");
        }
        [Theory]
        [IdentifierHttpStatusCodeErrorCodeMessagesHttpApiResponse]
        public void IdentifierHttpStatusCodeErrorCodeMessages_Successfully_HttpApiResponse(string identifier, HttpStatusCode statusCode, int errorCode,List<string> messages)
        {
            // Arrange
            HttpApiResponse<T> response = new HttpApiResponse<T>(identifier, statusCode,errorCode, messages);

            // Act
            var apiStatusCode = statusCode.GetApiStatusCode();
            // Assert

            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(apiStatusCode, response.StatusCode);
            Assert.Equal(messages, response.Messages);
            Assert.Equal(errorCode, response.ErrorCode);

            _testOutputHelper.WriteLine("Test passed!");
        }

    }
}
