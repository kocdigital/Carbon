using MassTransit;
using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public interface IRequestCarrierRequest : CorrelatedBy<Guid>
    {
        String RequestBody { get; set; }

        String SourceAddress { get; set; }
    }
}
