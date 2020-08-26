using System;
using Automatonymous;

namespace Carbon.MassTransit
{
    public abstract class CarbonStateMachineInstance : SagaStateMachineInstance
    {
        /// <summary>
        /// Carbon State Machine Instance Identifier
        /// </summary>
        /// <remarks>
        /// Should be unique for State Machine Instances ,an instance with different id means different instance.
        /// </remarks>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Current State of State Machine Instance
        /// </summary>
        /// <remarks>
        /// An instance can only be in one state at a given time.
        /// </remarks>
        public int CurrentState { get; set; }

        /// <summary>
        /// Completion Timeout Token Id of State Machine Instance
        /// </summary>
        /// <remarks>
        /// Token for the Completion Time out of StateMachineInstance
        /// </remarks>
        public Guid? CompletionTimeoutTokenId { get; set; }
    }
}
