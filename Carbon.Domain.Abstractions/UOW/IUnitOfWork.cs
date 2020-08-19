using System.Threading.Tasks;

namespace Carbon.Domain.Abstractions.UOW
{
    /// <summary>
    /// 	An interface that defines a Unit of Work operations for database contexts.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 	Saves the changes made to the context.
        /// </summary>
        /// <returns>
        ///     The number of state entries written to the database through the use of context.
        /// </returns>
        int SaveChanges();

        /// <summary>
        /// 	Saves the changes made to the context.
        /// </summary>
        /// <returns>
        ///     A task with result containing the number of state entries written to the database through the use of context.
        /// </returns>
        Task<int> SaveChangesAsync();
    }
}
