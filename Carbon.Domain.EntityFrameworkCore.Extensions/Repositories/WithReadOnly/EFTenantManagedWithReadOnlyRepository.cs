using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.Common.TenantManagementHandler.Interfaces;
using Carbon.Domain.Abstractions.Entities;
using Carbon.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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
    public abstract class EFTenantManagedWithReadOnlyRepository<TEntity, TContext, RContext> : EFTenantManagedRepository<TEntity,TContext>, IRepository<TEntity>
                                                                                    where TEntity : class, IEntity, IHaveOwnership<EntitySolutionRelation>
                                                                                    where TContext : DbContext
                                                                                    where RContext : DbContext, IReadOnlyContext

    {
        /// <summary>
        /// 	The database readonly context on which the repository will operate on.
        /// </summary>
        public readonly RContext readOnlyContext;


        /// <summary>
        /// 	Use this constructor when you want to use TenantManagement Filters by enablingSolutionFilter <paramref name="context"/>.
        /// 	<paramref name="enableSolutionFilter"/> should be true
        /// </summary>
        public EFTenantManagedWithReadOnlyRepository(TContext context, RContext rContext) : base(context)
        {
            this.readOnlyContext = rContext;
        }


        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object. </returns>
        public new virtual async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>().FindAsync(id, cancellationToken);
        }

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task which results in a list that contains all <typeparamref name="TEntity"/> objects in the database context.</returns>
        public new virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>().ToListAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves and returns the first <typeparamref name="TEntity"/> element that satisfies the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"> An expression that returns binary results for <typeparamref name="TEntity"/> objects. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns>The first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/> in the database.</returns>
        public new virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>().AsQueryable().FirstOrDefaultAsync(predicate, cancellationToken);
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

        /// <summary>
        /// Retrieves and returns all <typeparamref name="TEntity"/> objects from the database context.
        /// </summary>
        /// <param name="selector"> Expression to select properties to return. Null to return all properties. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task which results in a list that contains all <typeparamref name="TEntity"/> objects in the database context. </returns>
        public new virtual async Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>().Select(selector).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// If <paramref name="selector"/> is not null, returns the entity projected by the selector.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <param name="selector"> Optional selector to project the entity properties. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object or the projection if <paramref name="selector"/> is not null. </returns>
        public new virtual async Task<TResult> GetByIdAsync<TResult>(Guid id, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>()
                                .Where(e => EF.Property<Guid>(e, "Id") == id)
                                .Select(selector)
                                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
