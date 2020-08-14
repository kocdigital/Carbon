using Cassandra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.Cassandra.Abstractions
{
    public interface ICassandraUnitOfWork
    {
        Task ExecuteBatchStatementAsync(params Statement[] statements);
    }
}
