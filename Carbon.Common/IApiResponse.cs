using System.Collections.Generic;

namespace Carbon.Common
{
    /// <summary>
    /// An interface for Api's Responses that defines how responses should be
    /// </summary>
    public interface IApiResponse
    {
        /// <summary>
        /// Used for transport messages with Api Response
        /// </summary>
        /// <remarks>
        /// Generally used for <c>Exception</c> or <c>Validation</c> Messages
        /// </remarks>
        IList<string> Messages { get; }
        /// <summary>
        /// Indicates response's status <see cref="ApiStatusCode"/>
        /// </summary>
        ApiStatusCode StatusCode { get; }
        /// <summary>
        /// Used for supporting user defined/external error codes
        /// </summary>
        int? ErrorCode { get; }
        /// <summary>
        /// Indicades status. <c>True</c> for successfull and <c>False</c> for failed requests
        /// </summary>
        bool IsSuccess { get; }
        /// <summary>
        /// Used for request identification
        /// </summary>
        string Identifier { get; }
        /// <summary>
        /// Adds a message
        /// </summary>
        /// <param name="message">Message to add</param>
        void AddMessage(string message);
    }


}
