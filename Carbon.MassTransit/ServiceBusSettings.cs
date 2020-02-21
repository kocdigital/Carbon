using System;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;

namespace Carbon.MassTransit
{
    public class ServiceBusSettings : ServiceBusHostSettings
    {
        public string ConnectionString { get; set; }

        public ITokenProvider TokenProvider { get; set; }

        public TimeSpan OperationTimeout { get; set; }

        public TimeSpan RetryMinBackoff { get; set; }

        public TimeSpan RetryMaxBackoff { get; set; }

        public int RetryLimit { get; set; }

        public TransportType TransportType { get; set; }

        public Uri ServiceUri { get;  }
    }
}
