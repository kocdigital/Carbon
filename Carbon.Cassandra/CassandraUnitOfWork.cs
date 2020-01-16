using Carbon.Cassandra.Abstractions;
using System;
using Cassandra;
using System.Linq;
using System.Threading.Tasks;

namespace Carbon.Cassandra
{
    public class CassandraUnitOfWork : ICassandraUnitOfWork
    {
        private readonly ICassandraSessionFactory _cassandraSessionFactory;
        public CassandraUnitOfWork(ICassandraSessionFactory cassandraSessionFactory)
        {
            _cassandraSessionFactory = cassandraSessionFactory;
        }

        public void CheckStatement(params Statement[] statements)
        {
            if (!statements.Any())
            {
                throw new ArgumentNullException("statements","Statements cannot be null");
            }
        }
        public async Task ExecuteBatchStatementAsync(params Statement[] statements)
        {
            CheckStatement(statements);

            var statementGroups = statements.GroupBy(x => x.Keyspace);

            if (statementGroups.Count() > 1)
            {
                throw new NotSupportedException("Multiple keyspace batch statement does not supported");
            }

            var batch = new BatchStatement();

            foreach (var st in statements)
            {
                batch.Add(st);
            }

            await _cassandraSessionFactory.GetSession(statements.First().Keyspace).ExecuteAsync(batch).ConfigureAwait(false);
        }
    }
}
