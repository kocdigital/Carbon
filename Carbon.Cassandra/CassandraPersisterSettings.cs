using Carbon.Cassandra.Abstractions;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.Extensions.Options;
using Carbon.Cassandra.Abstractions;

namespace Carbon.Cassandra
{

    public class CassandraPersisterSettings : ICassandraPersisterSettings, IOptions<CassandraPersisterSettings>
    {
        public CassandraPersisterSettings Value => this;

        public int Port { get; set; }

        public CompressionType? CompressionSupportType { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

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

                if (!string.IsNullOrEmpty(settings.UserName))
                {
                    builder.WithAuthProvider(new PlainTextAuthProvider(settings.UserName, settings.Password));
                }

                #region Default Settings

                if(settings.CompressionSupportType is null)
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

    }
}
