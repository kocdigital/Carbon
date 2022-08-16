using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class ResponseFailed : IResponseFailed
    {
        public ResponseFailed(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
        public string ResponseBody { get; set; }
    }
}