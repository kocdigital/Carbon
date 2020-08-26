using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Carbon.MassTransit
{
    public abstract class CarbonSagaActivity<T> : Activity<T>
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
        public abstract Task Execute(BehaviorContext<T> context, Behavior<T> next);

        /// <summary>
        /// Saga execution of behavoir.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="context">Behavior Exception Context with T : instance and TException : acception type </param>
        /// <param name="next">Behavior to make fault handling </param>
        /// <returns></returns>
        public async Task Execute<T1>(BehaviorContext<T, T1> context, Behavior<T, T1> next)
        {
            await next.Execute(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Saga fault handling for next behavoir.
        /// </summary>
        /// <typeparam name="TException">Exception type to be thrown</typeparam>
        /// <param name="context">Behavior Exception Context with T : instance and TException : acception type </param>
        /// <param name="next">Behavior to make fault handling </param>
        /// <returns></returns>
        public Task Faulted<TException>(BehaviorExceptionContext<T, TException> context, Behavior<T> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Saga fault handling for next operation.
        /// </summary>
        /// <typeparam name="T1">Data type for Behavior Exception Context</typeparam>
        /// <typeparam name="TException">Exception type to be thrown</typeparam>
        /// <param name="context">Exceptional Behavior  Context with T : Instance and T1 : Data TException : acception type </param>
        /// <param name="next">Behavior to make fault handling </param>
        /// <returns></returns>
        public Task Faulted<T1, TException>(BehaviorExceptionContext<T, T1, TException> context, Behavior<T, T1> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Adds scope to Probe Context as "publisher"
        /// </summary>
        /// <param name="context">Greenpipes Probe Context to add scope</param>
        public void Probe(ProbeContext context)
        {
            context.CreateScope("publisher");
        }
    }
}
