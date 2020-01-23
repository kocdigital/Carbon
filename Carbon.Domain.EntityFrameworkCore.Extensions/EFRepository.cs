
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

    public abstract class EFRepository<TEntity, TKey, TContext> : IRepository<TEntity, TKey>
                                                                                    where TEntity : class, IEntity
                                                                                    where TContext : DbContext
    {
        public readonly TContext context;
        public EFRepository(TContext context)
        {
            this.context = context;
        }

        public Task<TEntity> AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> DeleteAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetByIdAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Query()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> QueryAsNoTracking()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        //public virtual async Task<TEntity> GetByIdAsync(Guid id)
        //{
        //    return await context.Set<TEntity>().FindAsync(id);
        //}

        //public virtual async Task<TEntity> AddAsync(TEntity entity)
        //{
        //    context.Set<TEntity>().Add(entity);
        //    await context.SaveChangesAsync();
        //    return entity;
        //}
        //public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        //{
        //    context.Entry(entity).State = EntityState.Modified;
        //    await context.SaveChangesAsync();
        //    return entity;
        //}

        //public virtual async Task<TEntity> DeleteAsync(Guid id)
        //{
        //    var entity = await context.Set<TEntity>().FindAsync(id);
        //    if (entity == null)
        //    {
        //        return entity;
        //    }

        //    context.Set<TEntity>().Remove(entity);
        //    await context.SaveChangesAsync();

        //    return entity;
        //}


        //public virtual async Task<List<TEntity>> GetAllAsync()
        //{
        //    return await context.Set<TEntity>().ToListAsync();
        //}


        //public virtual async Task<List<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entities)
        //{
        //    context.Set<TEntity>().AddRange(entities);
        //    await context.SaveChangesAsync();
        //    return entities.ToList();
        //}

        //public virtual async Task<List<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        //{
        //    context.Set<TEntity>().UpdateRange(entities);
        //    await context.SaveChangesAsync();
        //    return entities.ToList();
        //}

        //public virtual async Task<List<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities)
        //{
        //    context.Set<TEntity>().RemoveRange(entities);
        //    await context.SaveChangesAsync();
        //    return entities.ToList();
        //}

        //public virtual async Task<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        //{
        //    return await context.Set<TEntity>().AsQueryable().FirstOrDefaultAsync(predicate);
        //}

        //public virtual IQueryable<TEntity> Query()
        //{
        //    return context.Set<TEntity>();
        //}

        //public virtual IQueryable<TEntity> QueryAsNoTracking()
        //{
        //    using (new TransactionScope(
        //            TransactionScopeOption.Required,
        //            new TransactionOptions
        //            {
        //                IsolationLevel = IsolationLevel.ReadUncommitted
        //            }))
        //    {
        //        // query
        //        return context.Set<TEntity>().AsNoTracking();

        //    }
        //}

        //public async Task<int> SaveChangesAsync()
        //{
        //    return await context.SaveChangesAsync();
        //}
    }
}
