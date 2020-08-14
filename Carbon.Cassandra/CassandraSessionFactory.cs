using Carbon.Cassandra.Abstractions;
using System;
using Cassandra;
using System.Collections.Concurrent;

namespace Carbon.Cassandra
{
    public class CassandraSessionFactory : ICassandraSessionFactory
    {
        private readonly ConcurrentDictionary<string, Lazy<ISession>> _sessionCache;

        private readonly ICassandraPersisterSettings _cassandraPersisterSettings;

        public CassandraSessionFactory(ICassandraPersisterSettings cassandraPersisterSettings)
        {
            this._sessionCache = new ConcurrentDictionary<string, Lazy<ISession>>();
            this._cassandraPersisterSettings = cassandraPersisterSettings;
            this._sessionCache.GetOrAdd("defaultCluster", k => new Lazy<ISession>(() => this.CreateSession()));
        }

        public ISession GetSession(string keyspace)
        {
            return this._sessionCache.GetOrAdd(keyspace, k => new Lazy<ISession>(() => this.CreateSession(keyspace))).Value;
        }

        private ISession CreateSession(string keyspace)
        {
            if (string.IsNullOrEmpty(keyspace))
            {
                throw new ArgumentNullException("keyspace", message: $"Cannot find keyspace configuration named '{keyspace}'");
            }

            return this._cassandraPersisterSettings.Cluster.Connect(keyspace);
        }

        public ISession GetSessionFromDefaultCluster()
        {
            return this._sessionCache.GetOrAdd("defaultCluster", k => new Lazy<ISession>(() => this.CreateSession())).Value;
        }

        private ISession CreateSession()
        {
            return this._cassandraPersisterSettings.Cluster.Connect();
        }
        

    }
}
