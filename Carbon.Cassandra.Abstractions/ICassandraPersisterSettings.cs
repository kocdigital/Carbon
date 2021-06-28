using Cassandra;
using Cassandra.Mapping;

namespace Carbon.Cassandra.Abstractions
{
	public interface ICassandraPersisterSettings
	{

		string[] EndPoints { get; set; }

		int Port { get; set; }

		CompressionType? CompressionSupportType { get; set; }
		bool UsePasswordAuthenticator { get; set; }

		string UserName { get; set; }

		string Password { get; set; }

		string Keyspace { get; set; }

		Cluster Cluster { get; set; }

		IRetryPolicy RetryPolicy { get; set; }

		IReconnectionPolicy ReconnectionPolicy { get; set; }

		PoolingOptions PoolingOptions { get; set; }

		QueryOptions QueryOptions { get; set; }

		ILoadBalancingPolicy LoadBalancingPolicy { get; set; }

		void Build();

		void SetMapping(params Mappings[] mappings);
	}

}
