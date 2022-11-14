using MassTransit;
using System;
using static Carbon.MassTransit.AsyncReqResp.StaticHelpers;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public interface IResponseFailed : CorrelatedBy<Guid>
    {
        string ResponseBody { get; set; }
        ResponseCode ResponseCode { get; set; }
    }
}