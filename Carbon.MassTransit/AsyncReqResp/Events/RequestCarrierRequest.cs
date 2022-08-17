using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class RequestCarrierRequest : IRequestCarrierRequest
    {
        public RequestCarrierRequest(Guid correlationId, string requestBody)
        {
            CorrelationId = correlationId;
            RequestBody = requestBody;
            OriginatedRequestTime = DateTime.UtcNow;
        }

        public RequestCarrierRequest(Guid correlationId, string requestBody, DateTime originatedRequestTime) : this(correlationId, requestBody)
        {
            OriginatedRequestTime = originatedRequestTime;
        }

        public Guid CorrelationId { get; set; }
        public String RequestBody { get; set; }
        public DateTime OriginatedRequestTime { get; set; }
    }
}
