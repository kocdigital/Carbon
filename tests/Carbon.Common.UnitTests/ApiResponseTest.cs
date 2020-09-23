using Carbon.Test.Common.DataShares;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.Common.UnitTests
{
    public class DecimalFacts : ApiResponseTest<decimal> { public DecimalFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public class StringFacts : ApiResponseTest<string> { public StringFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public class DateTimeFacts : ApiResponseTest<DateTime> { public DateTimeFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public class TimSpanFacts : ApiResponseTest<TimeSpan> { public TimSpanFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public class Int32Facts : ApiResponseTest<int> { public Int32Facts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public class ObjectFacts : ApiResponseTest<object> { public ObjectFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public class ListStringFacts : ApiResponseTest<List<string>> { public ListStringFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
  
    public abstract class ApiResponseTest<T>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        protected ApiResponseTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        #region AddMessage
        [Theory]
        [AddValidMessageToApiResponse]
        public void AddMessageTo_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode, string message, int errorCode, IList<string> messages)
        {
            // Arrange
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode, errorCode, messages);

            // Act
            response.AddMessage(message);

            // Assert
            Assert.True(response.Messages.Any());
            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(errorCode, response.ErrorCode);
            Assert.Equal(messages, response.Messages);

            _testOutputHelper.WriteLine("Test passed!");
        }

        [Theory]
        [AddNullMessageToApiResponse]
        public void AddNullMessageTo_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode, string message, int errorCode, IList<string> messages)
        {
            // Arrange
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode, errorCode, messages);

            // Act
            response.AddMessage(message);

            // Assert
            Assert.Contains(response.Messages, x => x == null);
            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(errorCode, response.ErrorCode);
            Assert.Equal(messages, response.Messages);
  

            _testOutputHelper.WriteLine("Test passed!");
        }

        [Theory]
        [AddNullMessagesToApiResponse]
        public void AddNullMessagesTo_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode, int errorCode, params string[] messages)
        {
            // Arrange
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode, errorCode, messages);
            var messageCountBeforeInsert = response.Messages?.Count ?? 0;

            // Act
            response.AddMessages(messages);

            // Assert
            Assert.Equal(messageCountBeforeInsert, response.Messages.Count);
            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(errorCode, response.ErrorCode);

            _testOutputHelper.WriteLine("Test passed!");
        }

        [Theory]
        [AddValidMessagesToApiResponse]
        public void AddMessagesTo_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode, int errorCode, List<string> constMessages, params string[] messages)
        {
            // Arrange
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode, errorCode, constMessages);

            // Act
            response.AddMessages(messages);

            // Assert
            Assert.True(response.Messages.Any());
            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(errorCode, response.ErrorCode);
            Assert.True(response.Messages.Count > messages.Length);

            _testOutputHelper.WriteLine("Test passed!");
        }

        #endregion
    }
}
