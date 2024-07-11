using Carbon.Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.Domain.Abstractions.Repositories
{
    /// <summary>
    /// 	Defines a repository for an entity type.
    /// </summary>
    /// <typeparam name="T"> The entity type to operated worked on. </typeparam>
    public interface IRepository<T> where T : class, IEntity
    {
        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object. </returns>
        Task<T> GetByIdAsync(Guid id);
        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object. </returns>
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        ///     Creates and saves the given <typeparamref name="TEntity"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be saved to the database </param>
        /// <returns> A task that returns the <typeparamref name="TEntity"/> object saved to database. </returns>
        Task<T> CreateAsync(T entity);
        /// <summary>
        ///     Creates and saves the given <typeparamref name="TEntity"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be saved to the database </param>
        /// <returns> A task that returns the <typeparamref name="TEntity"/> object saved to database. </returns>
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken);

        /// <summary>
        ///     Updates and saves the given <typeparamref name="TEntity"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be updated to the database </param>
        /// <returns> A task that returns the <typeparamref name="TEntity"/> object updated in database. </returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        ///     Updates and saves the given <typeparamref name="TEntity"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be updated to the database </param>
        /// <returns> A task that returns the <typeparamref name="TEntity"/> object updated in database. </returns>
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// 	Deletes and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the specified <typeparamref name="TEntity"/> object. </param>
        /// <returns> A task whose result is the deleted <typeparamref name="TEntity"/> object. If no matching entry is found, returns null instead. </returns>
        Task<T> DeleteAsync(Guid id);

        /// <summary>
        /// 	Deletes and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the specified <typeparamref name="TEntity"/> object. </param>
        /// <returns> A task whose result is the deleted <typeparamref name="TEntity"/> object. If no matching entry is found, returns null instead. </returns>
        Task<T> DeleteAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <returns> A task which results in a list that contains all <typeparamref name="TEntity"/> objects in the database context.</returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <returns> A task which results in a list that contains all <typeparamref name="TEntity"/> objects in the database context.</returns>
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Creates and saves the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be created. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects created in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        Task<List<T>> CreateRangeAsync(IEnumerable<T> entities);

        /// <summary>
        ///     Creates and saves the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be created. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects created in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        Task<List<T>> CreateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        /// <summary>
        ///     Updates and saves the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be updated. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects updated in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        Task<List<T>> UpdateRangeAsync(IEnumerable<T> entities);

        /// <summary>
        ///     Updates and saves the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be updated. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects updated in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        Task<List<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        /// <summary>
        ///     Removes the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> from the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be deleted. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects deleted from the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        Task<List<T>> DeleteRangeAsync(IEnumerable<T> entities);

        /// <summary>
        ///     Removes the <typeparamref name="TEntity"/> objects in the given <code>IEnumerable</code> from the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be deleted. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="TEntity"/> objects deleted from the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        Task<List<T>> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        /// <summary>
        ///     Retrieves and returns the first <typeparamref name="TEntity"/> element that satisfies the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"> An expression that returns binary results for <typeparamref name="TEntity"/> objects. </param>
        /// <returns>The first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/> in the database.</returns>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        ///     Retrieves and returns the first <typeparamref name="TEntity"/> element that satisfies the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"> An expression that returns binary results for <typeparamref name="TEntity"/> objects. </param>
        /// <returns>The first <typeparamref name="TEntity"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/> in the database.</returns>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        ///     Returns a query on the <typeparamref name="TEntity"/> items in the database.
        /// </summary>
        /// <returns> A query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        IQueryable<T> Query();

        /// <summary>
        ///     Returns a query that does not track changes on the <typeparamref name="TEntity"/> items in the database.
        /// </summary>
        /// <returns> A no-tracking query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        /// <seealso cref="IQueryable{T}.AsNoTracking"/>
        IQueryable<T> QueryAsNoTracking();

        /// <summary>
        ///     Saves changes made to the database.
        /// </summary>
        /// <returns>A task that gives the number of state entries changed in the database as its result.</returns>
        Task<int> SaveChangesAsync();
        /// <summary>
        ///     Saves changes made to the database.
        /// </summary>
        /// <returns>A task that gives the number of state entries changed in the database as its result.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param> 
        /// <returns> A task which results in a list that contains all <typeparamref name="TEntity"/> objects in the database context.</returns> 
        Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector);


        /// <summary>
        ///     Retrieves and returns all <typeparamref name="TEntity"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> A task which results in a list that contains all <typeparamref name="TEntity"/> objects in the database context.</returns> 
        Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken);

        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// If <paramref name="selector"/> is not null, returns the entity projected by the selector.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <param name="selector"> Optional selector to project the entity properties. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object or the projection if <paramref name="selector"/> is not null. </returns>
        Task<TResult> GetByIdAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="TEntity"/> specified by <paramref name="id"/> from the database context.
        /// If <paramref name="selector"/> is not null, returns the entity projected by the selector.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="TEntity"/> object. </param>
        /// <param name="selector"> Optional selector to project the entity properties. </param>
        /// <param name="cancellationToken"> Token to monitor for cancellation requests. </param>
        /// <returns> A task whose result is the requested <typeparamref name="TEntity"/> object or the projection if <paramref name="selector"/> is not null. </returns>
        Task<TResult> GetByIdAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken);
    }
}
