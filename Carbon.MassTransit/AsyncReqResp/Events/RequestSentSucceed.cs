using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class RequestSentSucceed : IRequestSentSucceed
    {
        public RequestSentSucceed(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; set; }
    }
}