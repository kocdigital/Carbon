using System.Collections.Generic;

namespace Carbon.MassTransit.Abstractions
{
    public class MassTransitMessage<TType, TContent>
    {
        public MassTransitMessage()
        {

        }
        public MassTransitMessage(TContent content)
        {
            Content = content;
            MessageType = new List<string>() { $"urn:message:{typeof(TType).FullName.Replace(".", ":")}" };
        }
        public List<string> MessageType { get; set; }
        public TContent Content { get; set; }
    }
}
