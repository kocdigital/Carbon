using MassTransit;
using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class ResponseCarrier : IResponseCarrier
    {
        public string ResponseBody { get; set; }
        public StaticHelpers.ResponseCode ResponseCode { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
