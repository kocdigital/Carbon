using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class RequestCarrierRequest : IRequestCarrierRequest
    {
        public RequestCarrierRequest(Guid correlationId, string integrationBody)
        {
            CorrelationId = correlationId;
            RequestBody = integrationBody;
        }

        public Guid CorrelationId { get; set; }
        public String RequestBody { get; set; }
    }
}
