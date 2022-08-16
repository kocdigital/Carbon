using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class RequestStarterRequest : IRequestStarterRequest
    {
        public RequestStarterRequest()
        {

        }
        public RequestStarterRequest(Guid correlationId, string requestBody, string destinationEndpointName)
        {
            CorrelationId = correlationId;
            RequestBody = requestBody;
            DestinationEndpointName = destinationEndpointName;
        }

        public Guid CorrelationId { get; set; }
        public String RequestBody { get; set; }
        public String DestinationEndpointName { get; set; }
    }
}
