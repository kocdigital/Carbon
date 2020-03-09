using System.Collections.Generic;

namespace Carbon.MassTransit.Abstractions
{
    public class MassTransitMessage<T>
    {
        public MassTransitMessage(T message)
        {
            Message = message;
            MessageType = new List<string>() { $"urn:message:{typeof(T).Namespace.Replace(".", ":")}:{typeof(T).Name}" };
        }
        public List<string> MessageType { get; }
        public T Message { get; }
    }
}
