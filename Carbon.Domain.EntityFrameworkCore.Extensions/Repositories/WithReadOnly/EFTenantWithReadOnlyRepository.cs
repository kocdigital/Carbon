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
    /// 	Defines a repository for an entity that contains tenant information, and its related database context.
    /// </summary>
    /// <typeparam name="TEntity"> An entity type that contains tenant information. </typeparam>
    /// <typeparam name="TContext"> A database context containing records of <typeparamref name="TEntity"/> type entries. </typeparam>
    /// <seealso cref="IMustHaveTenant"/>
    /// <seealso cref="ITenantRepository{T}"/>
    public abstract class EFTenantWithReadOnlyRepository<TEntity, TContext, RContext> : EFTenantRepository<TEntity, TContext> where TEntity : class, IEntity, IMustHaveTenant
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
        public EFTenantWithReadOnlyRepository(TContext context, RContext rContext) : base(context)
        {
            this.readOnlyContext = rContext;
        }

        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> and <paramref name="tenantId"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <param name="tenantId"> Id of the tenant that is related to the <typeparamref name="TEntity"/> object. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object. </returns>
        public new async Task<TEntity> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
        }

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant whose related entities will be retrieved.</param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task which results in a list that contains the <typeparamref name="TEntity"/> objects related to the specified tenant.</returns>
        public new async Task<List<TEntity>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>().Where(x => x.TenantId == tenantId).ToListAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves and returns the first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that the entity is related to. </param>
        /// <param name="predicate"> An expression that returns binary results for <typeparamref name="TEntity"/> objects. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns>The first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/> in the database.</returns>
        public new async Task<TEntity> GetAsync(Guid tenantId, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>().AsQueryable().Where(x => x.TenantId == tenantId).FirstOrDefaultAsync(predicate, cancellationToken);
        }


        /// <summary>
        ///     Returns a query on the <typeparamref name="TEntity"/> items related to <paramref name="tenantId"/> in the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that is related to the entities. </param>
        /// <returns> A query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        public new IQueryable<TEntity> Query(Guid tenantId)
        {
            return readOnlyContext.Set<TEntity>().Where(c => c.TenantId == tenantId);
        }

        /// <summary>
        ///     Returns a query that does not track changes on the <typeparamref name="TEntity"/> items related to <paramref name="tenantId"/> in the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that is related to the entities. </param>
        /// <returns> A no-tracking query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        /// <seealso cref="IQueryable{T}.AsNoTracking"/>
        public new IQueryable<TEntity> QueryAsNoTracking(Guid tenantId)
        {
            using (new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadUncommitted
                    }))
            {
                // query
                return readOnlyContext.Set<TEntity>().Where(c => c.TenantId == tenantId).AsNoTracking();

            }
        } 

        /// <summary>
        /// Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant whose related entities will be retrieved.</param>
        /// <param name="selector"> Expression to select properties to return. Null to return all properties. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task which results in a list that contains the <typeparamref name="TEntity"/> objects related to the specified tenant. </returns>
        public new virtual async Task<List<TResult>> GetAllAsync<TResult>(Guid tenantId, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>().Where(x => x.TenantId == tenantId).Select(selector).ToListAsync();
        }

        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// If <paramref name="selector"/> is not null, returns the entity projected by the selector.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <param name="tenantId"> Id of the tenant. </param>
        /// <param name="selector"> Optional selector to project the entity properties. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object or the projection if <paramref name="selector"/> is not null. </returns>
        public new virtual async Task<TResult> GetByIdAsync<TResult>(Guid id, Guid tenantId, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken)
        {
            return await readOnlyContext.Set<TEntity>()
                                        .Where(x => EF.Property<Guid>(x, "Id") == id && EF.Property<Guid>(x, "TenantId") == tenantId)
                                        .Select(selector)
                                        .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
