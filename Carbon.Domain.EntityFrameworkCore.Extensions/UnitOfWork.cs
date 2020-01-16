using Carbon.Domain.Abstractions.UOW;
using System;
using System.Threading.Tasks;

namespace Carbon.Domain.EntityFrameworkCore
{
    public class UnitOfWork<T> : IUnitOfWork where T : CarbonContext<T>, IDisposable
    {
        private readonly T _context;
        public UnitOfWork(T context)
        {
            _context = context;
        }
        private bool _disposed = false;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

    }
}
