
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

    public abstract class EFRepository<TEntity, TContext> : IRepository<TEntity>
                                                                                    where TEntity : class, IEntity
                                                                                    where TContext : DbContext
    {
        public readonly TContext context;
        public EFRepository(TContext context)
        {
            this.context = context;
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> DeleteAsync(Guid id)
        {
            var entity = await context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }

            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync();

            return entity;
        }


        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await context.Set<TEntity>().ToListAsync();
        }


        public virtual async Task<List<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().AddRange(entities);
            await context.SaveChangesAsync();
            return entities.ToList();
        }

        public virtual async Task<List<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().UpdateRange(entities);
            await context.SaveChangesAsync();
            return entities.ToList();
        }

        public virtual async Task<List<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().RemoveRange(entities);
            await context.SaveChangesAsync();
            return entities.ToList();
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await context.Set<TEntity>().AsQueryable().FirstOrDefaultAsync(predicate);
        }

        public virtual IQueryable<TEntity> Query()
        {
            return context.Set<TEntity>();
        }

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

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

    }
}
