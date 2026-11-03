using MassTransit;

using System;
using System.Linq;

namespace Carbon.MassTransit.RoutingSlip
{
    public static class IServiceRoutingSlipExtensions
    {
        public static void ConsumeRoutingSlipActivity<TActivity, TArguments, TLogs>(this IRabbitMqBusFactoryConfigurator cfg,
                                       IServiceProvider provider, Action<IRabbitMqReceiveEndpointConfigurator> configurator = null)
            where TActivity : class, IActivity<TArguments, TLogs>
            where TArguments : class, CorrelatedBy<Guid>
            where TLogs : class, CorrelatedBy<Guid>
        {
            string compensationSuffix = "-faulty";
            string executionQueuePath = "rs-" + typeof(TArguments).Name.ToLowerInvariant();
            cfg.ReceiveEndpoint(executionQueuePath, e =>
              {
                  e.ExecuteActivityHost<TActivity, TArguments>(new Uri(e.InputAddress.AbsoluteUri + compensationSuffix), AsRegistrationContext(provider));
                  if (configurator != null)
                      configurator(e);
              });

            cfg.ReceiveEndpoint(executionQueuePath + compensationSuffix, e =>
            {
                e.CompensateActivityHost<TActivity, TLogs>(AsRegistrationContext(provider));
                if (configurator != null)
                    configurator(e);
            });
        }

        public static void ConsumeRoutingSlipExecuteOnlyActivity<TActivity, TArguments>(this IRabbitMqBusFactoryConfigurator cfg,
                                       IServiceProvider provider, Action<IRabbitMqReceiveEndpointConfigurator> configurator = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class, CorrelatedBy<Guid>
        {
            string executionQueuePath = "rs-" + typeof(TArguments).Name.ToLowerInvariant();
            cfg.ReceiveEndpoint(executionQueuePath, e =>
            {
                e.ExecuteActivityHost<TActivity, TArguments>(AsRegistrationContext(provider));
                if (configurator != null)
                    configurator(e);
            });
        }

        public static void ConsumeRoutingSlipActivity<TActivity, TArguments, TLogs>(this IServiceBusBusFactoryConfigurator cfg,
                                       IServiceProvider provider, Action<IServiceBusReceiveEndpointConfigurator> configurator = null)
            where TActivity : class, IActivity<TArguments, TLogs>
            where TArguments : class, CorrelatedBy<Guid>
            where TLogs : class, CorrelatedBy<Guid>
        {
            string compensationSuffix = "-faulty";
            string executionQueuePath = "rs-" + typeof(TArguments).Name.ToLowerInvariant();
            cfg.ReceiveEndpoint(executionQueuePath, e =>
            {
                e.ExecuteActivityHost<TActivity, TArguments>(new Uri(e.InputAddress.AbsoluteUri + compensationSuffix), AsRegistrationContext(provider));
                if (configurator != null)
                    configurator(e);
            });

            cfg.ReceiveEndpoint(executionQueuePath + compensationSuffix, e =>
            {
                e.CompensateActivityHost<TActivity, TLogs>(AsRegistrationContext(provider));
                if (configurator != null)
                    configurator(e);
            });
        }

        public static void ConsumeRoutingSlipExecuteOnlyActivity<TActivity, TArguments>(this IServiceBusBusFactoryConfigurator cfg,
                                       IServiceProvider provider, Action<IServiceBusReceiveEndpointConfigurator> configurator = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class, CorrelatedBy<Guid>
        {
            string executionQueuePath = "rs-" + typeof(TArguments).Name.ToLowerInvariant();
            cfg.ReceiveEndpoint(executionQueuePath, e =>
            {
                e.ExecuteActivityHost<TActivity, TArguments>(AsRegistrationContext(provider));
                if (configurator != null)
                    configurator(e);
            });
        }

        /// <summary>
        /// MassTransit 8 resolves activities through <see cref="IRegistrationContext"/>. The provider given to the bus
        /// configuration callbacks (AddRabbitMqBus / AddServiceBus) is already an <see cref="IBusRegistrationContext"/>.
        /// </summary>
        private static IRegistrationContext AsRegistrationContext(IServiceProvider provider)
        {
            if (provider is IRegistrationContext registrationContext)
                return registrationContext;

            return provider?.GetService(typeof(IBusRegistrationContext)) as IRegistrationContext
                ?? throw new ArgumentException("The provider must be the IBusRegistrationContext supplied by the bus configuration callback.", nameof(provider));
        }
    }
}
