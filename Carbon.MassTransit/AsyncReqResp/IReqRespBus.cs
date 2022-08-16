using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.MassTransit.AsyncReqResp
{
    public interface IReqRespRequestorBus : IBus
    {
    }

    public interface IReqRespResponderBus : IBus
    {
    }
}
