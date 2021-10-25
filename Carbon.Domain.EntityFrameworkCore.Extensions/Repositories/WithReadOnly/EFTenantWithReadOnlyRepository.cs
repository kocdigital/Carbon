using Carbon.Domain.Abstractions.Entities;
using Carbon.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object. </returns>
        public new async Task<TEntity> GetByIdAsync(Guid id, Guid tenantId)
        {
            return await readOnlyContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
        }
     

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant whose related entities will be retrieved.</param>
        /// <returns> A task which results in a list that contains the <typeparamref name="TEntity"/> objects related to the specified tenant.</returns>
        public new async Task<List<TEntity>> GetAllAsync(Guid tenantId)
        {
            return await readOnlyContext.Set<TEntity>().Where(x => x.TenantId == tenantId).ToListAsync();
        }

        /// <summary>
        ///     Retrieves and returns the first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that the entity is related to. </param>
        /// <param name="predicate"> An expression that returns binary results for <typeparamref name="TEntity"/> objects. </param>
        /// <returns>The first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/> in the database.</returns>
        public new async Task<TEntity> GetAsync(Guid tenantId, Expression<Func<TEntity, bool>> predicate)
        {
            return await readOnlyContext.Set<TEntity>().AsQueryable().Where(x => x.TenantId == tenantId).FirstOrDefaultAsync(predicate);
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
    }
}
