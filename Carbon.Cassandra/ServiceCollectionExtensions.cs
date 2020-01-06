using System;
using Carbon.Cassandra.Abstractions;
using Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Carbon.Cassandra.Abstractions;

namespace Carbon.Cassandra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCassandraPersister(this IServiceCollection services, IConfiguration configuration, Action<CassandraPersisterSettings> setupaction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupaction == null)
            {
                throw new ArgumentNullException(nameof(setupaction));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ICassandraPersisterSettings cassandraPersisterSettings = new CassandraPersisterSettings();
            configuration.GetSection("Cassandra").Bind(cassandraPersisterSettings);
            
            services.AddSingleton<ICassandraSessionFactory, CassandraSessionFactory>();

            services.AddOptions();
            services.AddSingleton(cassandraPersisterSettings);
            services.Configure(setupaction);
            setupaction?.Invoke(cassandraPersisterSettings as CassandraPersisterSettings);
            

            return services;
        }

        public static T SetEndPoints<T>(this T self, string[] endPoints)
            where T : ICassandraPersisterSettings
        {
            self.EndPoints = endPoints;
            return self;
        }

        public static T SetPort<T>(this T self, int port)
            where T : ICassandraPersisterSettings
        {
            self.Port = port;
            return self;
        }

        public static T SetAuthentication<T>(this T self, string username, string password)
            where T : ICassandraPersisterSettings
        {
            self.UserName = username;
            self.Password = password;
            return self;
        }

        public static T SetCompression<T>(this T self, CompressionType compressionType)
            where T : ICassandraPersisterSettings
        {
            self.CompressionSupportType = compressionType;
            return self;
        }

        public static T SetCluster<T>(this T self, Cluster cluster)
            where T : ICassandraPersisterSettings
        {
            self.Cluster = cluster;
            return self;
        }

        public static T SetReconnectionPolicy<T>(this T self, IReconnectionPolicy policy)
            where T : ICassandraPersisterSettings
        {
            self.ReconnectionPolicy = policy;
            return self;
        }

        public static T SetRetryPolicy<T>(this T self, IRetryPolicy policy)
            where T : ICassandraPersisterSettings
        {
            self.RetryPolicy = policy;
            return self;
        }

        public static T SetPoolingOptions<T>(this T self, PoolingOptions poolingOptions)
            where T : ICassandraPersisterSettings
        {
            self.PoolingOptions = poolingOptions;
            return self;
        }

        public static T SetQueryOptions<T>(this T self, QueryOptions queryOptions)
            where T : ICassandraPersisterSettings
        {
            self.QueryOptions = queryOptions;
            return self;
        }

        public static T SetLoadBalancingPolicy<T>(this T self, ILoadBalancingPolicy loadBalancingPolicy)
            where T : ICassandraPersisterSettings
        {
            self.LoadBalancingPolicy = loadBalancingPolicy;
            return self;
        }
    }
}
