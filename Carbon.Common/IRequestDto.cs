using System;

namespace Carbon.Common
{
    /// <summary>
    /// An interface for request
    /// </summary>
    public interface IRequestDto
    {
        /// <summary>
        /// A unique identifier value that is attached to requests and messages that allow reference to a particular transaction or event chain. 
        /// </summary>
        public Guid CorrelationId { get; set; }
    }
}
