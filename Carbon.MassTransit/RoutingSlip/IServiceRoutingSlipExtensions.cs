using GreenPipes;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.Courier;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus;
using MassTransit.MultiBus;
using MassTransit.RabbitMqTransport;
using MassTransit.Registration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using System;
using System.Linq;

using MassTransitNS = MassTransit;

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
                  e.ExecuteActivityHost<TActivity, TArguments>(new Uri(e.InputAddress.AbsoluteUri + compensationSuffix), provider);
                  if (configurator != null)
                      configurator(e);
              });

            cfg.ReceiveEndpoint(executionQueuePath + compensationSuffix, e =>
            {
                e.CompensateActivityHost<TActivity, TLogs>(provider);
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
                e.ExecuteActivityHost<TActivity, TArguments>(provider);
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
                e.ExecuteActivityHost<TActivity, TArguments>(new Uri(e.InputAddress.AbsoluteUri + compensationSuffix), provider);
                if (configurator != null)
                    configurator(e);
            });

            cfg.ReceiveEndpoint(executionQueuePath + compensationSuffix, e =>
            {
                e.CompensateActivityHost<TActivity, TLogs>(provider);
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
                e.ExecuteActivityHost<TActivity, TArguments>(provider);
                if (configurator != null)
                    configurator(e);
            });
        }

    }
}
