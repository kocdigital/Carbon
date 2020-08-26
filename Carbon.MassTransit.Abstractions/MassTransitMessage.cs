using System.Collections.Generic;

namespace Carbon.MassTransit.Abstractions
{
    /// <summary>
    /// Mass Transit Message Class
    /// </summary>
    /// <typeparam name="TType">Mass Transit User Type</typeparam>
    /// <typeparam name="TContent">Content Type</typeparam>
    public class MassTransitMessage<TType, TContent>
    {
        /// <summary>
        /// Constructor that initializes MassTransitMessage instance with no parameters
        /// </summary>
        public MassTransitMessage()
        {

        }
        /// <summary>
        /// Constructor that initializes MassTransitMessage instance with content of Generic <c>TType</c>
        /// </summary>
        public MassTransitMessage(TContent content)
        {
            Content = content;
            MessageType = new List<string>() { $"urn:message:{typeof(TType).FullName.Replace(".", ":")}" };
        }
        /// <summary>
        /// An array of message types, in a MessageUrn format, which can be deserialized.
        /// </summary>
        public List<string> MessageType { get; set; }
        /// <summary>
        /// Content of Message
        /// </summary>
        public TContent Content { get; set; }
    }
}
