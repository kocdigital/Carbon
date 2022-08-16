using MassTransit;
using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public interface IResponseFailed : CorrelatedBy<Guid>
    {
        string ResponseBody { get; set; }
    }
}