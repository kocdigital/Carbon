namespace Carbon.Cassandra.Abstractions
{
    public interface ICassandraReplicationStrategy
    {
        string CreateKeySpaceTemplate(string keySpace);
    }

}
