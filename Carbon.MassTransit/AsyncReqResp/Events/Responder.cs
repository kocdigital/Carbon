using MassTransit;
using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public class Responder : IResponder
    {
        public Responder()
        {

        }

        public Responder(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public string ResponseBody { get; set; }
        public StaticHelpers.ResponseCode ResponseCode { get; set; }

        public Guid CorrelationId { get; set; }

    }
}
