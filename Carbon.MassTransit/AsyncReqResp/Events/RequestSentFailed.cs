using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class RequestSentFailed : IRequestSentFailed
    {
        public RequestSentFailed(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }
}