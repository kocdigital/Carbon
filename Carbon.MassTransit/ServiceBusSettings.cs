using System;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;

namespace Carbon.MassTransit
{

    public class ServiceBusSettings : ServiceBusHostSettings
    {
        /// <summary>
        /// Connection String
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Token Provider
        /// </summary>
        public ITokenProvider TokenProvider { get; set; }
        /// <summary>
        /// Operation Timeout
        /// </summary>        
        public TimeSpan OperationTimeout { get; set; }
        /// <summary>
        /// Retry Minimum Backoff
        /// </summary>
        public TimeSpan RetryMinBackoff { get; set; }
        /// <summary>
        /// Retry Maximum Backoff
        /// </summary>
        public TimeSpan RetryMaxBackoff { get; set; }
        /// <summary>
        /// The retry limit for service bus operations
        /// </summary>
        public int RetryLimit { get; set; }
        /// <summary>
        /// TransportType of type <see cref="TransportType"/>
        /// </summary>
        public TransportType TransportType { get; set; }
        /// <summary>
        /// Service Uri
        /// </summary>
        public Uri ServiceUri { get;  }
    }
}
