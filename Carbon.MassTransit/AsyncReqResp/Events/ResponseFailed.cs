using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class ResponseFailed : IResponseFailed
    {
        public ResponseFailed(Guid correlationId, StaticHelpers.ResponseCode responseCode = StaticHelpers.ResponseCode.ServerError)
        {
            CorrelationId = correlationId;
            ResponseCode = responseCode;
        }

        public Guid CorrelationId { get; set; }
        public string ResponseBody { get; set; }
        public StaticHelpers.ResponseCode ResponseCode { get; set; }
    }
}