using Carbon.Common.UnitTests.DataShares;
using Carbon.Common.UnitTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.Common.UnitTests
{
    public class DecimalFacts : ApiResponseTest<decimal>, IClassFixture<ApiResponseTestFixture> { public DecimalFacts(ITestOutputHelper testOutputHelper, ApiResponseTestFixture apiResponseTestFixture) : base(testOutputHelper, apiResponseTestFixture, apiResponseTestFixture.DecimalSampleValue) { } }
    public class StringFacts : ApiResponseTest<string>, IClassFixture<ApiResponseTestFixture> { public StringFacts(ITestOutputHelper testOutputHelper, ApiResponseTestFixture apiResponseTestFixture) : base(testOutputHelper, apiResponseTestFixture, apiResponseTestFixture.StringSampleValue) { } }
    public class DateTimeFacts : ApiResponseTest<DateTime>, IClassFixture<ApiResponseTestFixture> { public DateTimeFacts(ITestOutputHelper testOutputHelper, ApiResponseTestFixture apiResponseTestFixture) : base(testOutputHelper, apiResponseTestFixture, apiResponseTestFixture.DateTimeSampleValue) { } }
    public class TimSpanFacts : ApiResponseTest<TimeSpan>, IClassFixture<ApiResponseTestFixture> { public TimSpanFacts(ITestOutputHelper testOutputHelper, ApiResponseTestFixture apiResponseTestFixture) : base(testOutputHelper, apiResponseTestFixture, apiResponseTestFixture.TimeSpanSampleValue) { } }
    public class Int32Facts : ApiResponseTest<int>, IClassFixture<ApiResponseTestFixture> { public Int32Facts(ITestOutputHelper testOutputHelper, ApiResponseTestFixture apiResponseTestFixture) : base(testOutputHelper, apiResponseTestFixture, apiResponseTestFixture.IntSpanSampleValue) { } }
    public class ObjectFacts : ApiResponseTest<object>, IClassFixture<ApiResponseTestFixture> { public ObjectFacts(ITestOutputHelper testOutputHelper, ApiResponseTestFixture apiResponseTestFixture) : base(testOutputHelper, apiResponseTestFixture, apiResponseTestFixture.ObjectSpanSampleValue) { } }
    public class ListStringFacts : ApiResponseTest<List<string>>, IClassFixture<ApiResponseTestFixture> { public ListStringFacts(ITestOutputHelper testOutputHelper, ApiResponseTestFixture apiResponseTestFixture) : base(testOutputHelper, apiResponseTestFixture, apiResponseTestFixture.ListStringSpanSampleValue) { } }

    public abstract class ApiResponseTest<T>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ApiResponseTestFixture _apiResponseTestFixture;
        private T Data { get; set; }
        protected ApiResponseTest(ITestOutputHelper testOutputHelper, ApiResponseTestFixture apiResponseTestFixture, T data)
        {
            _testOutputHelper = testOutputHelper;
            _apiResponseTestFixture = apiResponseTestFixture;
            Data = data;
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
        public void AddNullMessageTo_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode, string message, int errorCode)
        {
            // Arrange
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode, errorCode);

            // Act
            response.AddMessage(message);

            // Assert
            Assert.Contains(response.Messages, x => x == null);
            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(errorCode, response.ErrorCode);

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
        #region Constractor
        [Theory]
        [SetStatusAndIdentifierApiResponse]
        public void SetStatusAndIdentifier_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode)
        {
            // Arrange
            // Act
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode);

            // Assert
            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(statusCode, response.StatusCode);

            _testOutputHelper.WriteLine("Test passed!");
        }

        [Theory]
        [SetStatusIdentifierMessagesApiResponse]
        public void SetStatusIdentifierMessages_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode, IList<string> messages)
        {
            // Arrange
            // Act
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode, messages);

            // Assert
            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(messages, response.Messages);

            _testOutputHelper.WriteLine("Test passed!");
        }
        #endregion
        #region SetData
        [Theory]
        [SetStatusIdentifierMessagesApiResponse]
        public void SetData_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode, IList<string> messages)
        {
            // Arrange
            // Act
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode, messages);

            response.SetData(Data);
            // Assert
            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(messages, response.Messages);
            Assert.Equal(response.Data, Data);

            _testOutputHelper.WriteLine("Test passed!");
        }
        #endregion
        #region SetErrorCode
        [Theory]
        [SetStatusIdentifierMessagesApiResponse]
        public void SetErrorCode_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode, IList<string> messages)
        {
            // Arrange
            // Act
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode, messages);

            response.SetErrorCode(_apiResponseTestFixture.ErrorCodeSample);
            // Assert
            Assert.Equal(identifier, response.Identifier);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(messages, response.Messages);
            Assert.Equal(response.ErrorCode, _apiResponseTestFixture.ErrorCodeSample);

            _testOutputHelper.WriteLine("Test passed!");
        }
        #endregion
        #region IsSuccess
        [Theory]
        [IsSuccessTrueApiResponse]
        public void IsSuccessTrue_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode)
        {
            // Arrange
            // Act
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode);
            // Assert
            Assert.True(response.IsSuccess);
            _testOutputHelper.WriteLine("Test passed!");
        }
        [Theory]
        [IsSuccessFalseApiResponse]
        public void IsSuccessFalse_Successfully_ApiResponse(string identifier, ApiStatusCode statusCode, int errorCode)
        {
            // Arrange
            // Act
            ApiResponse<T> response = new ApiResponse<T>(identifier, statusCode, errorCode);
            // Assert
            Assert.False(response.IsSuccess);
            _testOutputHelper.WriteLine("Test passed!");
        }
        #endregion
    }
}
