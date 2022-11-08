using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class RequestStarterRequest : IRequestStarterRequest
    {
        public RequestStarterRequest()
        {

        }
        public RequestStarterRequest(Guid correlationId, string requestBody, string destinationEndpointName, Uri responseAddress = null)
        {
            CorrelationId = correlationId;
            RequestBody = requestBody;
            DestinationEndpointName = destinationEndpointName;
            ResponseAddress = responseAddress;
        }

        public Guid CorrelationId { get; set; }
        public String RequestBody { get; set; }
        public String DestinationEndpointName { get; set; }
        public Uri ResponseAddress { get; set; }
    }
}
