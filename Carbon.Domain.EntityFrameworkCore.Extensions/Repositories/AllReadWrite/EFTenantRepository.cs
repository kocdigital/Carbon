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
    public abstract class EFTenantRepository<TEntity, TContext> : ITenantRepository<TEntity> where TEntity : class, IEntity, IMustHaveTenant
                                                                                                         where TContext : DbContext
    {
        /// <summary>
        /// 	The database context on which the repository will operate on.
        /// </summary>
        public readonly TContext context;

        /// <summary>
        /// 	Constructor that initializes the working context as the given <paramref name="context"/>.
        /// </summary>
        public EFTenantRepository(TContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> and <paramref name="tenantId"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <param name="tenantId"> Id of the tenant that is related to the <typeparamref name="TEntity"/> object. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object. </returns>
        public async Task<TEntity> GetByIdAsync(Guid id, Guid tenantId)
        {
            return await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
        }

        /// <summary>
        ///     Creates and saves the given <typeparamref name="TEntity"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be saved to the database </param>
        /// <returns> A task that returns the <typeparamref name="TEntity"/> object saved to database. </returns>
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        ///     Updates and saves the given <typeparamref name="TEntity"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be updated to the database </param>
        /// <returns> A task that returns the <typeparamref name="TEntity"/> object updated in database. </returns>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// 	Deletes and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> and <paramref name="tenantId"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the specified <typeparamref name="TEntity"/> object. </param>
        /// <param name="tenantId"> Id of the tenant that is related to the <typeparamref name="TEntity"/> object. </param>
        /// <returns> A task whose result is the deleted <typeparamref name="TEntity"/> object. If no matching entry is found, returns null instead. </returns>
        public async Task<TEntity> DeleteAsync(Guid id, Guid tenantId)
        {
            var entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
            if (entity == null)
            {
                return entity;
            }

            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant whose related entities will be retrieved.</param>
        /// <returns> A task which results in a list that contains the <typeparamref name="TEntity"/> objects related to the specified tenant.</returns>
        public async Task<List<TEntity>> GetAllAsync(Guid tenantId)
        {
            return await context.Set<TEntity>().Where(x => x.TenantId == tenantId).ToListAsync();
        }

        /// <summary>
        ///     Creates and saves the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be created. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects created in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        public async Task<List<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().AddRange(entities);
            await context.SaveChangesAsync();
            return entities.ToList();
        }

        /// <summary>
        ///     Updates and saves the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be updated. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects updated in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        public async Task<List<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().UpdateRange(entities);
            await context.SaveChangesAsync();
            return entities.ToList();
        }

        /// <summary>
        ///     Removes the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> from the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be deleted. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects deleted from the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        public async Task<List<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().RemoveRange(entities);
            await context.SaveChangesAsync();
            return entities.ToList();
        }

        /// <summary>
        ///     Retrieves and returns the first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that the entity is related to. </param>
        /// <param name="predicate"> An expression that returns binary results for <typeparamref name="TEntity"/> objects. </param>
        /// <returns>The first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/> in the database.</returns>
        public async Task<TEntity> GetAsync(Guid tenantId, Expression<Func<TEntity, bool>> predicate)
        {
            return await context.Set<TEntity>().AsQueryable().Where(x => x.TenantId == tenantId).FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        ///     Returns a query on the <typeparamref name="TEntity"/> items related to <paramref name="tenantId"/> in the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that is related to the entities. </param>
        /// <returns> A query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        public IQueryable<TEntity> Query(Guid tenantId)
        {
            return context.Set<TEntity>().Where(c => c.TenantId == tenantId);
        }

        /// <summary>
        ///     Returns a query that does not track changes on the <typeparamref name="TEntity"/> items related to <paramref name="tenantId"/> in the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that is related to the entities. </param>
        /// <returns> A no-tracking query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        /// <seealso cref="IQueryable{T}.AsNoTracking"/>
        public IQueryable<TEntity> QueryAsNoTracking(Guid tenantId)
        {
            using (new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadUncommitted
                    }))
            {
                // query
                return context.Set<TEntity>().Where(c => c.TenantId == tenantId).AsNoTracking();

            }
        }

        /// <summary>
        ///     Saves changes made to the database.
        /// </summary>
        /// <returns>A task that gives the number of state entries changed in the database as its result.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }




    }
}
