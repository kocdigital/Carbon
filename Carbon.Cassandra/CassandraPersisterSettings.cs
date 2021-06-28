using Carbon.Cassandra.Abstractions;

using Cassandra;
using Cassandra.Mapping;

using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Carbon.Cassandra
{

	public class CassandraPersisterSettings : ICassandraPersisterSettings, IOptions<CassandraPersisterSettings>
	{
		public CassandraPersisterSettings Value => this;

		public int Port { get; set; }

		public CompressionType? CompressionSupportType { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }
		public bool UsePasswordAuthenticator { get; set; }

		public string[] EndPoints { get; set; }
		public string Keyspace { get; set; }

		Cluster ICassandraPersisterSettings.Cluster { get; set; }

		IRetryPolicy ICassandraPersisterSettings.RetryPolicy { get; set; }

		IReconnectionPolicy ICassandraPersisterSettings.ReconnectionPolicy { get; set; }

		PoolingOptions ICassandraPersisterSettings.PoolingOptions { get; set; }

		QueryOptions ICassandraPersisterSettings.QueryOptions { get; set; }

		ILoadBalancingPolicy ICassandraPersisterSettings.LoadBalancingPolicy { get; set; }

		public void SetMapping(params Mappings[] mappings)
		{
			MappingConfiguration.Global.Define(mappings);
		}

		public void Build()
		{
			var settings = this as ICassandraPersisterSettings;

			var cluster = settings.Cluster;

			if (cluster is null)
			{
				var builder = Cluster.Builder();

				builder.AddContactPoints(settings.EndPoints);

				builder.WithPort(settings.Port);

				if (settings.UsePasswordAuthenticator && !string.IsNullOrEmpty(settings.UserName) && !string.IsNullOrEmpty(settings.Password))
				{
					builder.WithCredentials(settings.UserName, settings.Password);					
					builder.WithSSL(getSSLOptions());
				}
				else if (!string.IsNullOrEmpty(settings.UserName))
				{
					builder.WithAuthProvider(new PlainTextAuthProvider(settings.UserName, settings.Password));
				}

				#region Default Settings

				if (settings.CompressionSupportType is null)
				{
					settings.SetCompression(CompressionType.Snappy);
				}

				if (settings.PoolingOptions is null)
				{
					settings.SetPoolingOptions(new PoolingOptions()
													.SetCoreConnectionsPerHost(HostDistance.Local, 3)
													.SetMaxSimultaneousRequestsPerConnectionTreshold(HostDistance.Local, 1500)
													.SetMaxConnectionsPerHost(HostDistance.Local, 8)
													.SetCoreConnectionsPerHost(HostDistance.Remote, 3)
													.SetHeartBeatInterval(6000)
													.SetMaxSimultaneousRequestsPerConnectionTreshold(HostDistance.Remote, 1500)
													.SetMaxConnectionsPerHost(HostDistance.Remote, 8));
				}

				if (settings.QueryOptions is null)
				{
					settings.SetQueryOptions(new QueryOptions().SetConsistencyLevel(ConsistencyLevel.One));
				}

				if (settings.RetryPolicy is null)
				{
					settings.SetRetryPolicy(DowngradingConsistencyRetryPolicy.Instance);
				}

				if (settings.ReconnectionPolicy is null)
				{
					settings.SetReconnectionPolicy(new ConstantReconnectionPolicy(1000));
				}

				if (settings.LoadBalancingPolicy is null)
				{
					settings.SetLoadBalancingPolicy(new RoundRobinPolicy());
				}

				#endregion

				builder.WithCompression(settings.CompressionSupportType.Value);

				builder.WithReconnectionPolicy(settings.ReconnectionPolicy);

				builder.WithRetryPolicy(settings.RetryPolicy);

				builder.WithPoolingOptions(settings.PoolingOptions);

				builder.WithQueryOptions(settings.QueryOptions);

				builder.WithLoadBalancingPolicy(settings.LoadBalancingPolicy);

				cluster = builder.Build();

				this.SetCluster<ICassandraPersisterSettings>(cluster);
			}
		}
		private SSLOptions getSSLOptions()
		{
			var settings = this as ICassandraPersisterSettings;
			var sslOptions = new SSLOptions(SslProtocols.Tls12, true, ValidateServerCertificate);
			//Some DNS servers (eg. our Azure DNS) do not support reverse-lookup
			//https://datastax-oss.atlassian.net/browse/CSHARP-652?page=com.atlassian.jira.plugin.system.issuetabpanels%3Aall-tabpanel
			var hostMap = new List<(string ip, string hostname)>();
			foreach (var host in settings.EndPoints)
			{
				hostMap.AddRange(Dns.GetHostAddresses(host).Select(ip => (ip.ToString(), host)));
			}
			sslOptions.SetHostNameResolver(ipAdd =>
			{
				var ip = ipAdd.ToString();
				foreach (var host in hostMap.Where(_ => _.ip == ip))
				{
					return host.hostname;
				}

				return ip;
			});
			return sslOptions;
		}

		private static bool ValidateServerCertificate(
			object sender,
			X509Certificate certificate,
			X509Chain chain,
			SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
				return true;

			// Do not allow this client to communicate with unauthenticated servers.
			return false;
		}
	}
}
