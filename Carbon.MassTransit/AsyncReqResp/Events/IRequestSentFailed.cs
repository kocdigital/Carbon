using MassTransit;
using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public interface IRequestSentFailed : CorrelatedBy<Guid>
    {
        string ErrorMessage { get; set; }
        string StackTrace { get; set; }
    }
}