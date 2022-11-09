using MassTransit;
using System;

namespace Carbon.MassTransit.AsyncReqResp.Events
{
    public interface IRequestStarterRequest : CorrelatedBy<Guid>
    {
        String RequestBody { get; set; }
        String DestinationEndpointName { get; set; }
        Uri ResponseAddress { get; set; }
    }
}
