using Cassandra;

namespace Carbon.Cassandra.Abstractions
{
    public interface ICassandraSessionFactory
    {
        ISession GetSessionFromDefaultCluster();
        ISession GetSession(string keyspace);
        
    }
}
