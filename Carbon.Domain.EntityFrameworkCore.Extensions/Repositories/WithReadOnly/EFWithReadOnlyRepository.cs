﻿using Carbon.Domain.Abstractions.Entities;
using Carbon.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace Carbon.Domain.EntityFrameworkCore
{
    /// <summary>
    /// 	Defines a repository for an entity and its related database context.
    /// </summary>
    /// <typeparam name="TEntity"> The entity type to operated worked on. </typeparam>
    /// <typeparam name="TContext"> A database context containing records of <typeparamref name="TEntity"/> type entries. </typeparam>
    /// <seealso cref="IRepository{T}"/>
    public abstract class EFWithReadOnlyRepository<TEntity, TContext, RContext> : EFRepository<TEntity, TContext>
                                                                                    where TEntity : class, IEntity
                                                                                    where TContext : DbContext
                                                                                    where RContext : DbContext, IReadOnlyContext

    {
        /// <summary>
        /// 	The database context on which the repository will operate on.
        /// </summary>
        public readonly RContext readOnlyContext;

        /// <summary>
        /// 	Constructor that initializes the working context as the given <paramref name="context"/>.
        /// </summary>
        public EFWithReadOnlyRepository(TContext context, RContext rContext) : base(context)
        {
            this.readOnlyContext = rContext;
        }

        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object. </returns>
        public new virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await readOnlyContext.Set<TEntity>().FindAsync(id);
        }

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <returns> A task which results in a list that contains all <typeparamref name="TEntity"/> objects in the database context.</returns>
        public new virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await readOnlyContext.Set<TEntity>().ToListAsync();
        }

        /// <summary>
        ///     Retrieves and returns the first <typeparamref name="TEntity"/> element that satisfies the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"> An expression that returns binary results for <typeparamref name="TEntity"/> objects. </param>
        /// <returns>The first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/> in the database.</returns>
        public new virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await readOnlyContext.Set<TEntity>().AsQueryable().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        ///     Returns a query on the <typeparamref name="TEntity"/> items in the database.
        /// </summary>
        /// <returns> A query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        public new virtual IQueryable<TEntity> Query()
        {
            return readOnlyContext.Set<TEntity>();
        }

        /// <summary>
        ///     Returns a query that does not track changes on the <typeparamref name="TEntity"/> items in the database.
        /// </summary>
        /// <returns> A no-tracking query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        /// <seealso cref="IQueryable{T}.AsNoTracking"/>
        public new virtual IQueryable<TEntity> QueryAsNoTracking()
        {
            using (new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadUncommitted
                    }))
            {
                return readOnlyContext.Set<TEntity>().AsNoTracking();

            }
        }
    }
}
