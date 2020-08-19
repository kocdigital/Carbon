using Carbon.Domain.Abstractions.UOW;
using System;
using System.Threading.Tasks;

namespace Carbon.Domain.EntityFrameworkCore
{
    /// <summary>
    /// 	A class that defines a Unit of Work operations for the given context.
    /// </summary>
    /// <typeparam name="T"> Type of context to be used </typeparam>
    /// <seealso cref = "CarbonContext{TContext}" />
    /// <seealso cref = "IDisposable" />
    public class UnitOfWork<T> : IUnitOfWork where T : CarbonContext<T>, IDisposable
    {
        /// <summary>
        /// 	Context on which the UnitOfWork will be operating on.
        /// </summary>
        private readonly T _context;

        /// <summary>
        /// 	Constructor that initializes the working context as the given <paramref name="context"/>.
        /// </summary>
        public UnitOfWork(T context)
        {
            _context = context;
        }

        /// <summary>
        /// 	Boolean value representing whether the resources have been disposed or not.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 	Disposes the currently used context if <paramref name="disposing"/> is true. Otherwise does nothing.
        /// </summary>
        /// <param name = "disposing">A boolean value signaling whether the disposing operation should take place.</typeparam>
        /// <seealso cref = "CarbonContext{TContext}" />
        /// <seealso cref = "IDisposable" />
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }  
            }
            this._disposed = true;
        }

        /// <summary>
        /// 	Disposes the currently used context.
        /// </summary>
        /// <seealso cref = "CarbonContext{TContext}" />
        /// <seealso cref = "IDisposable" />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 	Saves the changes made to the context.
        /// </summary>
        /// <returns>
        ///     The number of state entries written to the database through the use of context.
        /// </returns>
        /// <seealso cref = "CarbonContext{TContext}" />
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <summary>
        /// 	Saves the changes made to the context.
        /// </summary>
        /// <returns>
        ///     A task with result containing the number of state entries written to the database through the use of context.
        /// </returns>
        /// <seealso cref = "CarbonContext{TContext}" />
        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

    }
}
