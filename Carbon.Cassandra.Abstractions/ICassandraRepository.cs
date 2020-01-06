using Cassandra;
using Cassandra.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Carbon.Cassandra.Abstractions
{
    public interface ICassandraRepository<T>
    where T : class
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        T FindOne(Expression<Func<T, bool>> query);
        Task<IEnumerable<T>> FindAsync(string cql, params object[] values);
        Task<T> FindOneAsync(Expression<Func<T, bool>> query);
        IEnumerable<T> Find(Expression<Func<T, bool>> query);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> query);
        void Delete(Expression<Func<T, bool>> query);
        Task DeleteAsync(Expression<Func<T, bool>> query);
        void Add(T item);
        Task AddAsync(T item);
        void AddRange(List<T> items);
        Task AddRangeAsync(List<T> items);
        void Update(T item);
        Task UpdateAsync(T item);
        RowSet Execute(string cql);
        Task ExecuteAsync(IStatement statement);
        Task<BoundStatement> SetStatement(string cql, params object[] values);
        IPage<T> FindPaging(string cql, int limit, int offset, params object[] values);
        IPage<T> GetNextPage(string cql, int limit, byte[] pagingState, params object[] values);
    }

}
