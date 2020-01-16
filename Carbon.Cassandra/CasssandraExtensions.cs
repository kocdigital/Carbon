using Cassandra.Data.Linq;
using Carbon.Cassandra.Abstractions;
using Cassandra;

namespace Carbon.Cassandra
{
    public static class CasssandraExtensions
    {
        internal static void CreateKeyspace(this ISession session, ICassandraReplicationStrategy replicationStrategy, string keyspace)
        {
            var createKeySpaceQuery = replicationStrategy.CreateKeySpaceTemplate(keyspace);

            session.Execute(createKeySpaceQuery);

            session.ChangeKeyspace(keyspace);
        }

        public static void CreateKeySpaceIfNotExists(this ISession session, string keySpaceName)
        {
            session.CreateKeyspaceIfNotExists(keySpaceName);
        }

        public static void CreateTableIfNotExists<T>(this ISession session, string keySpaceName)
            where T : class
        {
            session.ChangeKeyspace(keySpaceName);

            var table = new Table<T>(session);

            table.CreateIfNotExists();
        }

        public static void CreateUserDefineTypes(this ISession session, string keySpaceName, UdtMap[] udtMaps)
        {
            session.ChangeKeyspace(keySpaceName);
            
            session.UserDefinedTypes.Define(udtMaps);
        }

        public static PreparedStatement PrepareFormat(this ISession session, string cqlFormatString, params object[] args)
        {
            return session.Prepare(cqlQuery: string.Format(cqlFormatString, args));
        }
    }
}
