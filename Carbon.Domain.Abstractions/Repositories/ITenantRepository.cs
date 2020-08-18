using Carbon.Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Carbon.Domain.Abstractions.Repositories
{
    /// <summary>
    /// 	Defines a repository for an entity that contains tenant information, and its related database context.
    /// </summary>
    /// <typeparam name="T"> An entity type that contains tenant information. </typeparam>
    /// <seealso cref="IMustHaveTenant"/>
    public interface ITenantRepository<T> where T : IMustHaveTenant
    {
        /// <summary>
        /// 	Retrieves and returns the <typeparamref name="T"/> specified by <paramref name="id"/> and <paramref name="tenantId"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the requested <typeparamref name="T"/> object. </param>
        /// <param name="tenantId"> Id of the tenant that is related to the <typeparamref name="T"/> object. </param>
        /// <returns> A task whose result is the requested <typeparamref name="T"/> object. </returns>
        Task<T> GetByIdAsync(Guid id, Guid tenantId);

        /// <summary>
        ///     Creates and saves the given <typeparamref name="T"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be saved to the database </param>
        /// <returns> A task that returns the <typeparamref name="T"/> object saved to database. </returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        ///     Updates and saves the given <typeparamref name="T"/> object in the database.
        /// </summary>
        /// <param name="entity"> Object to be updated to the database </param>
        /// <returns> A task that returns the <typeparamref name="T"/> object updated in database. </returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// 	Deletes and returns the <typeparamref name="T"/> specified by <paramref name="id"/> and <paramref name="tenantId"/> from the database context.
        /// </summary>
        /// <param name="id"> Id of the specified <typeparamref name="T"/> object. </param>
        /// <param name="tenantId"> Id of the tenant that is related to the <typeparamref name="T"/> object. </param>
        /// <returns> A task whose result is the deleted <typeparamref name="T"/> object. If no matching entry is found, returns null instead. </returns>
        Task<T> DeleteAsync(Guid id, Guid tenantId);

        /// <summary>
        ///     Retrieves and returns all <typeparamref name="T"/> objects related to the given <paramref name="tenantId"/> from the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant whose related entities will be retrieved.</param>
        /// <returns> A task which results in a list that contains the <typeparamref name="T"/> objects related to the specified tenant.</returns>
        Task<List<T>> GetAllAsync(Guid tenantId);



        /// <summary>
        ///     Creates and saves the <typeparamref name="T"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be created. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="T"/> objects created in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        Task<List<T>> CreateRangeAsync(IEnumerable<T> entities);

        /// <summary>
        ///     Updates and saves the <typeparamref name="T"/> objects in the given <code>IEnumerable</code> to the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be updated. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="T"/> objects updated in the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        Task<List<T>> UpdateRangeAsync(IEnumerable<T> entities);

        /// <summary>
        ///     Removes the <typeparamref name="T"/> objects in the given <code>IEnumerable</code> from the database.
        /// </summary>
        /// <param name="entities"> The collection that contains the entities to be deleted. </param>
        /// <returns>A task which results in a list that contains the <typeparamref name="T"/> objects deleted from the database.</returns>
        /// <seealso cref="IEnumerable{T}"/>
        Task<List<T>> DeleteRangeAsync(IEnumerable<T> entities);



        /// <summary>
        ///     Retrieves and returns the first <typeparamref name="T"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that the entity is related to. </param>
        /// <param name="predicate"> An expression that returns binary results for <typeparamref name="T"/> objects. </param>
        /// <returns>The first <typeparamref name="T"/> element that is related to <paramref name="tenantId"/> and also satisfies the given <paramref name="predicate"/> in the database.</returns>
        Task<T> GetAsync(Guid tenantId, Expression<Func<T, bool>> predicate);

        /// <summary>
        ///     Returns a query on the <typeparamref name="T"/> items related to <paramref name="tenantId"/> in the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that is related to the entities. </param>
        /// <returns> A query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        IQueryable<T> Query(Guid tenantId);

        /// <summary>
        ///     Returns a query that does not track changes on the <typeparamref name="T"/> items related to <paramref name="tenantId"/> in the database.
        /// </summary>
        /// <param name="tenantId"> Id of the tenant that is related to the entities. </param>
        /// <returns> A no-tracking query on the entities related to the given tenant in the database.</returns>
        /// <seealso cref="IQueryable"/>
        /// <seealso cref="IQueryable{T}.AsNoTracking"/>
        IQueryable<T> QueryAsNoTracking(Guid tenantId);

        /// <summary>
        ///     Saves changes made to the database.
        /// </summary>
        /// <returns>A task that gives the number of state entries changed in the database as its result.</returns>
        Task<int> SaveChangesAsync();
    }
}
