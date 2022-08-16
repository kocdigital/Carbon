using MassTransit;
using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public interface IResponseSucceed : CorrelatedBy<Guid>
    {
        string ResponseBody { get; set; }
    }
}