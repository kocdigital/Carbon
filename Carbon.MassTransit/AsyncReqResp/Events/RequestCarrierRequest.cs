using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class RequestCarrierRequest : IRequestCarrierRequest
    {
        public RequestCarrierRequest(Guid correlationId, string requestBody, string sourceAddress)
        {
            CorrelationId = correlationId;
            RequestBody = requestBody;
            OriginatedRequestTime = DateTime.UtcNow;
            SourceAddress = sourceAddress;
        }

        public RequestCarrierRequest(Guid correlationId, string requestBody, DateTime originatedRequestTime, string sourceAddress) : this(correlationId, requestBody, sourceAddress)
        {
            OriginatedRequestTime = originatedRequestTime;
        }

        public Guid CorrelationId { get; set; }
        public String RequestBody { get; set; }
        public DateTime OriginatedRequestTime { get; set; }
        public string SourceAddress { get; set; }
    }
}
