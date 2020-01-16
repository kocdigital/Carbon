using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Carbon.Cassandra.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Carbon.Cassandra
{
    public abstract class BaseCassandraRepository<T> : ICassandraRepository<T>
    where T : class
    {
        private readonly ICassandraSessionFactory sessionFactory;

        protected BaseCassandraRepository(ICassandraSessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public abstract string Keyspace { get; }

        public IEnumerable<T> GetAll()
        {
            var table = new Table<T>(this.sessionFactory.GetSession(this.Keyspace));
            return table.Execute().ToList();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            var table = new Table<T>(this.sessionFactory.GetSession(this.Keyspace));
            return table.ExecuteAsync();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> query)
        {
            var table = new Table<T>(this.sessionFactory.GetSession(this.Keyspace));

            return table.Where(query).Execute();
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> query)
        {
            var table = new Table<T>(this.sessionFactory.GetSession(this.Keyspace));
            return table.Where(query).ExecuteAsync();
        }

        public Task<IEnumerable<T>> FindAsync(string cql, params object[] values)
        {
            var result = new Mapper(this.sessionFactory.GetSession(this.Keyspace)).FetchAsync<T>(cql, values);
            return result;
        }

        public T FindOne(Expression<Func<T, bool>> query)
        {
            var table = new Table<T>(this.sessionFactory.GetSession(this.Keyspace));
            return table.Where(query).FirstOrDefault().Execute();
        }

        public Task<T> FindOneAsync(Expression<Func<T, bool>> query)
        {
            var table = new Table<T>(this.sessionFactory.GetSession(this.Keyspace));
            return table.Where(query).FirstOrDefault().ExecuteAsync();
        }


        public void Delete(Expression<Func<T, bool>> query)
        {
            var table = new Table<T>(this.sessionFactory.GetSession(this.Keyspace));
            table.Select(u => u).Where(query).Delete().Execute();
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> query)
        {
            var table = new Table<T>(this.sessionFactory.GetSession(this.Keyspace));
            await table.Where(query).Delete().ExecuteAsync();
        }

        public void Add(T item)
        {
            new Mapper(this.sessionFactory.GetSession(this.Keyspace)).Insert<T>(item);
        }

        public async Task AddAsync(T item)
        {
            await new Mapper(this.sessionFactory.GetSession(this.Keyspace)).InsertAsync(item).ConfigureAwait(false);
        }

        public void AddRange(List<T> items)
        {
            foreach (var item in items)
            {
                new Mapper(this.sessionFactory.GetSession(this.Keyspace)).Insert<T>(item);
            }
        }

        public async Task AddRangeAsync(List<T> items)
        {
            foreach (var item in items)
            {
                await new Mapper(this.sessionFactory.GetSession(this.Keyspace)).InsertAsync(item).ConfigureAwait(false);
            }
        }

        public void Update(T item)
        {
            new Mapper(this.sessionFactory.GetSession(this.Keyspace)).Update<T>(item);
        }

        public Task UpdateAsync(T item)
        {
            return new Mapper(this.sessionFactory.GetSession(this.Keyspace)).UpdateAsync<T>(item);
        }

        public RowSet Execute(string cql)
        {
            return this.sessionFactory.GetSession(this.Keyspace).Execute(cql);
        }

        public async Task ExecuteAsync(IStatement statement)
        {
            await this.sessionFactory.GetSession(this.Keyspace).ExecuteAsync(statement).ConfigureAwait(false);
        }

        public async Task<BoundStatement> SetStatement(string cql, params object[] values)
        {
            var statement = await this.sessionFactory.GetSession(this.Keyspace).PrepareAsync(cql);
            var boundStatement = statement.Bind(values);
            return boundStatement;
        }

        //This will be removed
        public IPage<T> FindPaging(string cql, int limit, int offset, params object[] values)
        {
            IPage<T> result;
            byte[] pagingState = null;
            var safeCounter = 1;
            var pageNumber = 1;
            var totalpage = limit;

            while (offset > totalpage)
            {
                totalpage += limit;
                pageNumber++;
            }
            do
            {
                result = new Mapper(this.sessionFactory
                        .GetSession(this.Keyspace))
                        .FetchPage<T>(Cql.New(cql, values)
                        .WithOptions(opt => opt.SetPageSize(limit)
                                            .SetPagingState(pagingState)));
                pagingState = result.PagingState;

            } while (pagingState != null && safeCounter++ < pageNumber);

            return result;
        }

        public IPage<T> GetNextPage(string cql, int limit, byte[] pagingState, params object[] values)
        {
            var result = new Mapper(this.sessionFactory
                            .GetSession(this.Keyspace))
                            .FetchPage<T>(Cql.New(cql, values)
                            .WithOptions(opt => opt.SetPageSize(limit)
                                                .SetPagingState(pagingState)));
            return result;
        }
    }
}
