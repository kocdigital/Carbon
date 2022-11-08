using MassTransit;
using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public interface IResponder : CorrelatedBy<Guid>
    {
        String ResponseBody { get; set; }
        StaticHelpers.ResponseCode ResponseCode { get; set; }
    }

    
}
