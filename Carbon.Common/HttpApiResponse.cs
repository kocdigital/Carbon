using System.Collections.Generic;
using System.Net;

namespace Carbon.Common
{
    /// <summary>
    /// Adaptaion of <see cref="ApiResponse{T}"/> for satisfiying http requests
    /// </summary>
    /// <typeparam name="T">Indicates response's data type. <see cref="ApiResponse{T}.Data"/></typeparam>
    public class HttpApiResponse<T> : ApiResponse<T>
    {
        /// <summary>
        /// Constructor that initializes an HttpApiResponse with identifier and status code
        /// </summary>
        /// <param name="identifier"><see cref="IApiResponse.Identifier"/></param>
        /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
        /// <remarks><param name="statusCode"> equivalent to <see cref="IApiResponse.StatusCode"/>. Keeping mind <see cref="ApiStatusCode"/> While using </remarks>
        public HttpApiResponse(string identifier, HttpStatusCode statusCode) : base(identifier, statusCode.GetApiStatusCode())
        {
           
        }
        /// <summary>
        /// Constructor that initializes an HttpApiResponse with identifier, status code and error code
        /// </summary>
        /// <param name="identifier"><see cref="IApiResponse.Identifier"/></param>
        /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
        /// <param name="errorCode"><see cref="IApiResponse.ErrorCode"/></param>
        /// <remarks><param name="statusCode"> equivalent to <see cref="IApiResponse.StatusCode"/>. Keeping mind <see cref="ApiStatusCode"/> While using </remarks>
        public HttpApiResponse(string identifier, HttpStatusCode statusCode, int errorCode) : base(identifier, statusCode.GetApiStatusCode(), errorCode)
        {
        }
        /// <summary>
        /// Constructor that initializes an HttpApiResponse with identifier, status code and messages
        /// </summary>
        /// <param name="identifier"><see cref="IApiResponse.Identifier"/></param>
        /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
        /// <param name="messages"><see cref="IApiResponse.Messages"/></param>
        /// <remarks><param name="statusCode"> equivalent to <see cref="IApiResponse.StatusCode"/>. Keeping mind <see cref="ApiStatusCode"/> While using </remarks>
        public HttpApiResponse(string identifier, HttpStatusCode statusCode, IList<string> messages) : base(identifier, statusCode.GetApiStatusCode(), messages)
        {
        }
        /// <summary>
        /// Constructor that initializes an HttpApiResponse with identifier, status code, error code and messages
        /// </summary>
        /// <param name="identifier"><see cref="IApiResponse.Identifier"/></param>
        /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
        /// <param name="errorCode"><see cref="IApiResponse.ErrorCode"/></param>
        /// <param name="messages"><see cref="IApiResponse.Messages"/></param>
        /// <remarks><param name="statusCode"> equivalent to <see cref="IApiResponse.StatusCode"/>. Keeping mind <see cref="ApiStatusCode"/> While using </remarks>
        public HttpApiResponse(string identifier, HttpStatusCode statusCode, int errorCode, IList<string> messages) : base(identifier, statusCode.GetApiStatusCode(), errorCode, messages)
        {
        }
    }


}
