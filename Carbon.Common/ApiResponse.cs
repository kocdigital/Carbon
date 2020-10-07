using System.Collections.Generic;

namespace Carbon.Common
{
    /// <summary>
    /// Creates a api response with given type
    /// </summary>
    /// <typeparam name="T">Indicates response's data type. <see cref="ApiResponse{T}.Data"/></typeparam>
    public class ApiResponse<T> : IApiResponse
    {
        /// <summary>
        /// Constructor that initializes an ApiResponse with identifier and status code
        /// </summary>
        /// <param name="identifier"><see cref="IApiResponse.Identifier"/></param>
        /// <param name="statusCode"><see cref="IApiResponse.StatusCode"/></param>
        public ApiResponse(string identifier, ApiStatusCode statusCode)
        {
            StatusCode = statusCode;
            Identifier = identifier;
        }
        /// <summary>
        /// Constructor that initializes an ApiResponse with identifier, status code and error code
        /// </summary>
        /// <param name="identifier"><see cref="IApiResponse.Identifier"/></param>
        /// <param name="statusCode"><see cref="IApiResponse.StatusCode"/></param>
        /// <param name="errorCode"><see cref="IApiResponse.ErrorCode"/></param>
        public ApiResponse(string identifier, ApiStatusCode statusCode, int errorCode) : this(identifier, statusCode)
        {
            ErrorCode = errorCode;
        }
        /// <summary>
        /// Constructor that initializes an ApiResponse with identifier, status code and messages
        /// </summary>
        /// <param name="identifier"><see cref="IApiResponse.Identifier"/></param>
        /// <param name="statusCode"><see cref="IApiResponse.StatusCode"/></param>
        /// <param name="messages"><see cref="IApiResponse.Messages"/></param>
        public ApiResponse(string identifier, ApiStatusCode statusCode, IList<string> messages) : this(identifier, statusCode)
        {
            Messages = messages;
        }
        /// <summary>
        /// Constructor that initializes an ApiResponse with identifier, status code, error code and messages
        /// </summary>
        /// <param name="identifier"><see cref="IApiResponse.Identifier"/></param>
        /// <param name="statusCode"><see cref="IApiResponse.StatusCode"/></param>
        /// <param name="errorCode"><see cref="IApiResponse.ErrorCode"/></param>
        /// <param name="messages"><see cref="IApiResponse.Messages"/></param>
        public ApiResponse(string identifier, ApiStatusCode statusCode, int errorCode, IList<string> messages) : this(identifier, statusCode, errorCode)
        {
            Messages = messages;
        }
        /// <summary>
        /// Contains response data from type of <typeparamref name="T"/>
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// <see cref="IApiResponse.Messages"/>
        /// </summary>
        public IList<string> Messages { get; set; }
        /// <summary>
        /// <see cref="IApiResponse.ErrorCode"/>
        /// </summary>
        public int? ErrorCode { get; set; }
        /// <summary>
        /// <see cref="IApiResponse.StatusCode"/>
        /// </summary>
        public ApiStatusCode StatusCode { get; set; }
        /// <summary>
        /// Indicades status. <c>True</c> for successfull and <c>False</c> for failed requests.
        /// </summary>
        /// <remarks>
        /// If ApiResponse contains <see cref="ErrorCode"/> or <see cref="StatusCode"/> differs from <c>ApiStatusCode.OK</c>
        /// </remarks>
        public bool IsSuccess
        {
            get
            {
                return ErrorCode == null && StatusCode == ApiStatusCode.OK;
            }
        }
        /// <summary>
        /// <see cref="IApiResponse.Identifier"/>
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Sets response data with given type <typeparamref name="T"/>. <see cref="Data"/>
        /// </summary>
        /// <param name="data"></param>
        public void SetData(T data)
        {
            Data = data;
        }
        /// <summary>
        /// Adds a message to <see cref="Messages"/>.
        /// </summary>
        /// <param name="message">Message to add</param>
        /// <remarks>If <see cref="Messages"/> is null, creates a new list for it.</remarks>
        public void AddMessage(string message)
        {
            if (Messages == null)
                Messages = new List<string>();

            Messages.Add(message);
        }
        /// <summary>
        /// Adds multiple messages to <see cref="Messages"/>.
        /// </summary>
        /// <param name="messages">Messages to add</param>
        /// <remarks>If <see cref="Messages"/> is null, creates a new list for it.</remarks>
        public void AddMessages(params string[] messages)
        {
            if (Messages == null)
                Messages = new List<string>();

            if (messages != null)
            {
                foreach (var msg in messages)
                {
                    Messages.Add(msg);
                }
            }

        }
        /// <summary>
        /// Sets error code. <see cref="ErrorCode"/>
        /// </summary>
        /// <param name="errorCode"></param>
        public void SetErrorCode(int errorCode)
        {
            ErrorCode = errorCode;
        }

    }


}
