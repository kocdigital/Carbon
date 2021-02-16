﻿using System;
using Automatonymous;

namespace Carbon.MassTransit
{
    public abstract class CarbonStateMachineInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public Guid? CompletionTimeoutTokenId { get; set; }
    }
}
