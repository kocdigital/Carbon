using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Carbon.MassTransit
{
    /// <summary>
    /// Carbon Saga Activity Class
    /// </summary>
    /// <remarks>
    /// An activity is part of a behavoir than is executed in order
    /// </remarks>
    /// <typeparam name="T">Type of Instance</typeparam>
    public abstract class CarbonSagaActivity<T> : IStateMachineActivity<T>
        where T : class, SagaStateMachineInstance
    {
        private readonly ILogger<T> _logger;
        private readonly ConsumeContext _context;

        /// <summary>
        /// CarbonSagaActivity Constructor
        /// </summary>
        /// <param name="logger">Microsoft logger for logging</param>
        /// <param name="context">Mass Transit consume context for consuming incoming messages</param>
        public CarbonSagaActivity(ILogger<T> logger, ConsumeContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Accepts the State Machine Visitor to visit caller of method
        /// </summary>
        /// <param name="visitor">Visitor of Activity</param>
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Saga execution of operation abstact method
        /// </summary>
        /// <param name="context">Behavior Exception Context with T : instance and TException : acception type </param>
        /// <param name="next">Next Behavior after fault occured</param>
        /// <returns></returns>
        public abstract Task Execute(BehaviorContext<T> context, IBehavior<T> next);

        /// <summary>
        /// Saga execution of behavoir for an event carrying data.
        /// </summary>
        /// <remarks>
        /// MassTransit 8 invokes this overload for activities bound to events with data (e.g. <c>When(SomeEvent).Activity(x =&gt; x.OfInstanceType&lt;TActivity&gt;())</c>).
        /// MassTransit 7 (Automatonymous) used to widen the behavior and call <see cref="Execute(BehaviorContext{T}, IBehavior{T})"/> instead,
        /// so the call is forwarded to keep derived activities working unchanged.
        /// </remarks>
        /// <typeparam name="T1">Event data type</typeparam>
        /// <param name="context">Behavior Context with T : instance and T1 : event data</param>
        /// <param name="next">Next behavior in the chain</param>
        /// <returns></returns>
        public Task Execute<T1>(BehaviorContext<T, T1> context, IBehavior<T, T1> next)
            where T1 : class
        {
            return Execute(context, new WidenBehavior<T1>(next, context));
        }

        /// <summary>
        /// Saga fault handling for behavoir.
        /// </summary>
        /// <typeparam name="TException">Exception type to be thrown</typeparam>
        /// <param name="context">Behavior Exception Context with T : instance and TException : acception type </param>
        /// <param name="next">Behavior to make fault handling </param>
        /// <returns></returns>
        public Task Faulted<TException>(BehaviorExceptionContext<T, TException> context, IBehavior<T> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Saga fault handling for behavoir.
        /// </summary>
        /// <typeparam name="T1">Data type for Behavior Exception Context</typeparam>
        /// <typeparam name="TException">Exception type to be thrown</typeparam>
        /// <param name="context">Exceptional Behavior  Context with T : Instance and T1 : Data TException : acception type </param>
        /// <param name="next">Behavior to make fault handling </param>
        /// <returns></returns>
        public Task Faulted<T1, TException>(BehaviorExceptionContext<T, T1, TException> context, IBehavior<T, T1> next)
            where T1 : class
            where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Adds scope to Probe Context as "publisher"
        /// </summary>
        /// <param name="context">MassTransit.ProbeContext to add scope<see cref="ProbeContext"/></param>
        public void Probe(ProbeContext context)
        {
            context.CreateScope("publisher");
        }

        /// <summary>
        /// Adapts the typed next behavior (<see cref="IBehavior{T, T1}"/>) to <see cref="IBehavior{T}"/>,
        /// continuing the chain with the original typed context.
        /// </summary>
        private sealed class WidenBehavior<T1> : IBehavior<T>
            where T1 : class
        {
            private readonly IBehavior<T, T1> _next;
            private readonly BehaviorContext<T, T1> _context;

            public WidenBehavior(IBehavior<T, T1> next, BehaviorContext<T, T1> context)
            {
                _next = next;
                _context = context;
            }

            public void Accept(StateMachineVisitor visitor)
            {
                _next.Accept(visitor);
            }

            public void Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public Task Execute(BehaviorContext<T> context)
            {
                return _next.Execute(_context);
            }

            public Task Execute<TData>(BehaviorContext<T, TData> context)
                where TData : class
            {
                return _next.Execute(_context);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<T, TException> context)
                where TException : Exception
            {
                return Task.FromException(context.Exception);
            }

            public Task Faulted<TData, TException>(BehaviorExceptionContext<T, TData, TException> context)
                where TData : class
                where TException : Exception
            {
                return Task.FromException(context.Exception);
            }
        }
    }
}
