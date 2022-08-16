using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class ResponseSucceed : IResponseSucceed
    {
        public ResponseSucceed(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
        public string ResponseBody { get; set; }
    }
}