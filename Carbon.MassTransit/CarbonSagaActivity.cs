using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;
using Microsoft.Extensions.Logging;

namespace Carbon.MassTransit
{
    public abstract class CarbonSagaActivity<T> : Activity<T>
    {
        private readonly ILogger<T> _logger;
        public CarbonSagaActivity(ILogger<T> logger)
        {
            _logger = logger;
        }
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public abstract Task Execute(BehaviorContext<T> context, Behavior<T> next);

        public async Task Execute<T1>(BehaviorContext<T, T1> context, Behavior<T, T1> next)
        {
            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<T, TException> context, Behavior<T> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T1, TException>(BehaviorExceptionContext<T, T1, TException> context, Behavior<T, T1> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("publisher");
        }
    }
}
