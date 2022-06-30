using System;
using Azure;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using MassTransit.Azure.ServiceBus.Core;

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
        /// <summary>
        /// Service Uri
        /// </summary>
        public Uri ServiceUri { get; }

        public ServiceBusClient ServiceBusClient { get; set; }

        public ServiceBusAdministrationClient ServiceBusAdministrationClient { get; set; }

        public AzureNamedKeyCredential NamedKeyCredential { get; set; }

        public AzureSasCredential SasCredential { get; set; }

        public TokenCredential TokenCredential { get; set; }

        public ServiceBusTransportType TransportType { get; set; }
    }
}
