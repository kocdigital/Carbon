using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.Common.TenantManagementHandler.Interfaces;
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
    /// 	Defines a repository for an entity and its related database context.
    /// </summary>
    /// <typeparam name="TEntity"> The entity type to operated worked on. </typeparam>
    /// <typeparam name="TContext"> A database context containing records of <typeparamref name="TEntity"/> type entries. </typeparam>
    /// <seealso cref="IRepository{T}"/>
    public abstract class EFTenantFilterManagedRepository<TEntity, TContext> : TenantManagementFilteredRepositoryBase, IRepository<TEntity>
                                                                                    where TEntity : class, IEntity, IHaveOwnership<EntitySolutionRelation>
                                                                                    where TContext : DbContext
    {
        /// <summary>
        /// 	The database context on which the repository will operate on.
        /// </summary>
        public readonly TContext context;


        /// <summary>
        /// 	Use this constructor when you want to use TenantManagement Filters by enablingSolutionFilter <paramref name="context"/>.
        /// 	<paramref name="enableSolutionFilter"/> should be true
        /// </summary>
        public EFTenantFilterManagedRepository(TContext context) : base(context)
        {
                this.context = context;
        }

        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object. </returns>
        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }

        /// <summary>
        ///     Creates and saves the given <typeparamref name="TEntity"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be saved to the database </param>
        /// <returns> A task that returns the <typeparamref name="TEntity"/> object saved to database. </returns>
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync();
            await base.ConnectToSolution(entity);
            return entity;
        }

        /// <summary>
        ///     Updates and saves the given <typeparamref name="TEntity"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be updated to the database </param>
        /// <returns> A task that returns the <typeparamref name="TEntity"/> object updated in database. </returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            await base.ConnectToSolution(entity);
            return entity;
        }

        /// <summary>
        /// 	Deletes and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the specified <typeparamref name="TEntity"/> object. </param>
        /// <returns> A task whose result is the deleted <typeparamref name="TEntity"/> object. If no matching entry is found, returns null instead. </returns>
        public virtual async Task<TEntity> DeleteAsync(Guid id)
        {
            var entity = await context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }

            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync();

            await base.RemoveSolutions(entity);

            return entity;
        }

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <returns> A task which results in a list that contains all <typeparamref name="TEntity"/> objects in the database context.</returns>
        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await context.Set<TEntity>().ToListAsync();
        }

        /// <summary>
        ///     Creates and saves the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be created. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects created in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        public virtual async Task<List<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().AddRange(entities);
            await context.SaveChangesAsync();

            foreach (var entity in entities)
            {
                await base.ConnectToSolution(entity);
            }

            return entities.ToList();
        }

        /// <summary>
        ///     Updates and saves the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be updated. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects updated in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        public virtual async Task<List<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().UpdateRange(entities);
            await context.SaveChangesAsync();

            foreach (var entity in entities)
            {
                await base.ConnectToSolution(entity);
            }

            return entities.ToList();
        }

        /// <summary>
        ///     Removes the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> from the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be deleted. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects deleted from the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        public virtual async Task<List<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().RemoveRange(entities);
            await context.SaveChangesAsync();

            foreach (var entity in entities)
            {
                await base.RemoveSolutions(entity);
            }

            return entities.ToList();
        }

        /// <summary>
        ///     Retrieves and returns the first <typeparamref name="TEntity"/> element that satisfies the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"> An expression that returns binary results for <typeparamref name="TEntity"/> objects. </param>
        /// <returns>The first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/> in the database.</returns>
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await context.Set<TEntity>().AsQueryable().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        ///     Returns a query on the <typeparamref name="TEntity"/> items in the database.
        /// </summary>
        /// <returns> A query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        public virtual IQueryable<TEntity> Query()
        {
            return context.Set<TEntity>();
        }

        /// <summary>
        ///     Returns a query that does not track changes on the <typeparamref name="TEntity"/> items in the database.
        /// </summary>
        /// <returns> A no-tracking query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        /// <seealso cref="IQueryable{T}.AsNoTracking"/>
        public virtual IQueryable<TEntity> QueryAsNoTracking()
        {
            using (new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadUncommitted
                    }))
            {
                return context.Set<TEntity>().AsNoTracking();

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
