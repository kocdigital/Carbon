
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
    public abstract class EFTenantRepository<TEntity, TContext> : ITenantRepository<TEntity> where TEntity : class, IEntity, IMustHaveTenant
                                                                                                         where TContext : DbContext
    {
        public readonly TContext context;
        public EFTenantRepository(TContext context)
        {
            this.context = context;
        }

        public async Task<TEntity> GetByIdAsync(Guid id, Guid tenantId)
        {
            return await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

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

        public async Task<List<TEntity>> GetAllAsync(Guid tenantId)
        {
            return await context.Set<TEntity>().Where(x => x.TenantId == tenantId).ToListAsync();
        }

        public async Task<List<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().AddRange(entities);
            await context.SaveChangesAsync();
            return entities.ToList();
        }

        public async Task<List<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().UpdateRange(entities);
            await context.SaveChangesAsync();
            return entities.ToList();
        }


        public async Task<List<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().RemoveRange(entities);
            await context.SaveChangesAsync();
            return entities.ToList();
        }

        public async Task<TEntity> Get(Guid tenantId, Expression<Func<TEntity, bool>> predicate)
        {
            return await context.Set<TEntity>().AsQueryable().Where(x => x.TenantId == tenantId).FirstOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> Query(Guid tenantId)
        {
            return context.Set<TEntity>().Where(c => c.TenantId == tenantId);
        }

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

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }




    }
}
