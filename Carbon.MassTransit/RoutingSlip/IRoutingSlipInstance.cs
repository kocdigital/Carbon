using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.MassTransit.RoutingSlip
{
    public interface IRoutingSlipInstance : CorrelatedBy<Guid>
    {

    }
}
