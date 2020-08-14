using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using MassTransit.RabbitMqTransport;

namespace Carbon.MassTransit
{
    public class RabbitMqSettings : RabbitMqHostSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string VirtualHost { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ushort Heartbeat { get; set; }

        public bool Ssl { get; set; }

        public SslProtocols SslProtocol { get; set; }

        public string SslServerName { get; set; }

        public SslPolicyErrors AcceptablePolicyErrors { get; set; }

        public string ClientCertificatePath { get; set; }

        public string ClientCertificatePassphrase { get; set; }

        public X509Certificate ClientCertificate { get; set; }

        public bool UseClientCertificateAsAuthenticationIdentity { get; set; }

        public LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }
        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }

        public string[] ClusterMembers { get; set; }

        public IRabbitMqEndpointResolver HostNameSelector { get; set; }

        public string ClientProvidedName { get; set; }

        public Uri HostAddress { get; set; }

        public bool PublisherConfirmation { get; set; }

        public ushort RequestedChannelMax { get; set; }

        public int RequestedConnectionTimeout { get; set; }
    }
}
