using Carbon.Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Carbon.Domain.Abstractions.Repositories
{

    public interface IRepository<T> where T : class, IEntity
    {
        Task<T> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(Guid id);
        Task<List<T>> GetAllAsync();


        Task<List<T>> CreateRangeAsync(IEnumerable<T> entities);
        Task<List<T>> UpdateRangeAsync(IEnumerable<T> entities);
        Task<List<T>> DeleteRangeAsync(IEnumerable<T> entities);


        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query();
        IQueryable<T> QueryAsNoTracking();
        Task<int> SaveChangesAsync();
    }
}
