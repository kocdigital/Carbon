using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using MassTransit.RabbitMqTransport;

namespace Carbon.MassTransit
{
    /// <summary>
    /// Rabbit Mq Settings Class
    /// </summary>
    public class RabbitMqSettings : RabbitMqHostSettings
    {
        /// <summary>
        /// Rabbit MQ Host Address
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Rabbit MQ Port Number
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Rabbit MQ Virtual Host Address
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// Rabbit MQ user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Rabbit MQ password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Heart beat of Rabbit MQ <c>Default:</c> 60
        /// </summary>
        public TimeSpan Heartbeat { get; set; }

        /// <summary>
        /// Value <c>true</c> if SSL is used
        /// </summary>
        public bool Ssl { get; set; }

        /// <summary>
        /// SSL Protocol Possible Version Definition <see href="https://docs.microsoft.com/en-us/dotnet/api/system.security.authentication.sslprotocols?view=netcore-3.1"/>
        /// </summary>
        public SslProtocols SslProtocol { get; set; }

        /// <summary>
        /// SSL Server Name 
        /// </summary>
        public string SslServerName { get; set; }

        /// <summary>
        /// Acceptable Policy Errors <see cref="SslPolicyErrors"/>
        /// </summary>
        public SslPolicyErrors AcceptablePolicyErrors { get; set; }
        /// <summary>
        /// Client Certificate Path
        /// </summary>
        public string ClientCertificatePath { get; set; }
        /// <summary>
        /// Client Certificate Pass phrase 
        /// </summary>
        public string ClientCertificatePassphrase { get; set; }
        /// <summary>
        /// ClientCertificate <see cref="X509Certificate"/>
        /// </summary>
        public X509Certificate ClientCertificate { get; set; }
        /// <summary>
        /// Uses client certificate as Authentication Identity if <c>true</c>
        /// </summary>
        public bool UseClientCertificateAsAuthenticationIdentity { get; set; }
        /// <summary>
        /// Certificate Selection Callback method delegate
        /// </summary>
        public LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }
        /// <summary>
        /// Certificate Validation Callback method delegate
        /// </summary>
        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }
        /// <summary>
        /// Certificate Validation Callback method delegate
        /// </summary>
        public string[] ClusterMembers { get; set; }
        /// <summary>
        /// Host Name Selector od type <see cref="IRabbitMqEndpointResolver"/>
        /// </summary>
        public IRabbitMqEndpointResolver EndpointResolver { get; set; }
        /// <summary>
        /// Provided Name for client
        /// </summary>
        public string ClientProvidedName { get; set; }
        /// <summary>
        /// Host Address Uri
        /// </summary>
        public Uri HostAddress { get; set; }
        /// <summary>
        /// Publisher Confirmation
        /// </summary>
        /// <remarks>
        /// Is <c>true</c> if Publisher Confirmation is OK
        /// </remarks>
        public bool PublisherConfirmation { get; set; }
        /// <summary>
        /// <c>MAX</c> Requested Channel Count
        /// </summary>
        public ushort RequestedChannelMax { get; set; }
        /// <summary>
        /// Requested Connection Timeout time in <c>msec</c>
        /// </summary>
        public TimeSpan RequestedConnectionTimeout { get; set; }
        /// <summary>
        /// Prefetch Count
        /// </summary>
        public ushort PrefetchCount { get; set; }

        public BatchSettings BatchSettings { get; set; }

        public TimeSpan ContinuationTimeout { get; set; }
    }
}
