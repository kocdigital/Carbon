using MassTransit;
using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public interface IRequestSentSucceed : CorrelatedBy<Guid>
    {
    }
}