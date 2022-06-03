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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Carbon.MassTransit.RoutingSlip
{
    public static class IRoutingSlipBuilder
    {
        public static readonly string InstanceName = "CarbonRoutingSlipInstance";
        public static void AddActivity<TArguments>(this RoutingSlipBuilder builder,
                                       IBusControl busControl, TArguments arguments = null)
            where TArguments : class, CorrelatedBy<Guid>
        {
            string executionQueuePath = "rs-" + typeof(TArguments).Name.ToLowerInvariant();
            string name = "name-" + typeof(TArguments).Name.ToLowerInvariant();
            Uri daUri = new Uri($"rabbitmq://{busControl.Address.Host}/{executionQueuePath}");

            if (arguments == null)
            {
                builder.AddActivity(name, daUri);
            }
            else
            {
                builder.AddActivity(name, daUri, arguments);
            }
        }

        public static void SetInstance<TInstance>(this RoutingSlipBuilder builder, TInstance instance)
            where TInstance : class, IRoutingSlipInstance

        {
            var instanceData = JsonConvert.SerializeObject(instance);
            builder.AddVariable(InstanceName, instanceData);
        }

        public static TInstance GetInstance<TInstance, TArguments>(this ExecuteContext<TArguments> context) 
            where TInstance : class, IRoutingSlipInstance 
            where TArguments: class, CorrelatedBy<Guid>
        {
            try
            {
                if(context.Message.Variables.TryGetValue(InstanceName, out object instanceValue))
                {
                    var daConfig = JsonConvert.DeserializeObject<TInstance>(instanceValue.ToString());
                    return daConfig;
                }
                else
                {
                    return default(TInstance);
                }
            }
            catch
            {
                return default(TInstance);
            }
        }

        public static TInstance GetInstance<TInstance, TLogs>(this CompensateContext<TLogs> context)
            where TInstance : class, IRoutingSlipInstance
            where TLogs : class, CorrelatedBy<Guid>
        {
            try
            {
                if (context.Message.Variables.TryGetValue(InstanceName, out object instanceValue))
                {
                    var daConfig = JsonConvert.DeserializeObject<TInstance>(instanceValue.ToString());
                    return daConfig;
                }
                else
                {
                    return default(TInstance);
                }
            }
            catch
            {
                return default(TInstance);
            }
        }

        public static ExecutionResult CompletedWithInstanceUpdate<TInstance, TArguments, TLogs>(this ExecuteContext<TArguments> context, TLogs log, TInstance instance)
            where TInstance : class, IRoutingSlipInstance
            where TArguments : class, CorrelatedBy<Guid>
            where TLogs : class, CorrelatedBy<Guid>
        {
            var instanceData = JsonConvert.SerializeObject(instance);
            return context.CompletedWithVariables(log, new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(InstanceName, instanceData) });
        }

        public static ExecutionResult CompletedWithInstanceUpdate<TInstance, TArguments>(this ExecuteContext<TArguments> context, TInstance instance)
            where TInstance : class, IRoutingSlipInstance
            where TArguments : class, CorrelatedBy<Guid>
        {
            var instanceData = JsonConvert.SerializeObject(instance);
            return context.CompletedWithVariables(new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(InstanceName, instanceData) });
        }
    }
}
