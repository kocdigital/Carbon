using Carbon.Common;
using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.Domain.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Domain.EntityFrameworkCore
{
    public static class EFExtensions
    {
        public static async Task<ICollection<T>> ToEntityListIncludeOwnershipAsync<T, U>(this IQueryable<T> query, DbContext ctx)
           where T : IHaveOwnership<U>, IEntity
           where U : class, IOwnerRelation
        {
            var grouped = query.GroupJoin(ctx.Set<U>(), k => k.Id, k1 => k1.EntityId, (k, k1) => new RelationEntity<T, U> { Entity = k, Relations = k1.ToList() });
            var groupedList = await grouped.ToListAsync();
            groupedList.ForEach(k => k.Entity.RelationalOwners = k.Relations);
            return groupedList.Select(k => k.Entity).ToList();
        }

        public static IQueryable<RelationEntityPair<T, U>> IncludeSolutionRelation<T, U>(this IQueryable<T> query, DbContext ctx)
           where T : IHaveOwnership<U>, IEntity
           where U : class, IOwnerRelation, ISoftDelete
        {
            //var daList = query.Join(ctx.Set<U>(), k => k.Id, k1 => k1.EntityId, (k, k1) => new RelationEntityPair<T, U> { Entity = k, Relation = k1 });
            var daList = (from q in query
                          join io in ctx.Set<U>().Where(k => !k.IsDeleted) on q.Id equals io.EntityId
                          into ljoin
                          from lj in ljoin.DefaultIfEmpty()
                          select new RelationEntityPair<T, U> { Entity = q, Relation = lj });

            return daList;
        }

        public static async Task<T> EntityFirstOrDefaultIncludeOwnershipAsync<T, U>(this IQueryable<T> query, DbContext ctx)
            where T : IHaveOwnership<U>, IEntity
            where U : class, IOwnerRelation
        {
            var grouped = query.GroupJoin(ctx.Set<U>(), k => k.Id, k1 => k1.EntityId, (k, k1) => new RelationEntity<T, U> { Entity = k, Relations = k1.ToList() });

            var groupedList = await grouped.ToListAsync();
            var groupedFirst = groupedList.FirstOrDefault();

            if (groupedFirst == null)
                return default(T);

            groupedFirst.Entity.RelationalOwners = groupedFirst.Relations;
            return groupedFirst.Entity;
        }

        //public static async Task<RelationEntity<T, U>> FirstOrDefaultIncludeOwnershipAsync<T, U>(this IQueryable<T> query, DbContext ctx)
        //    where T : IHaveOwnership<U>, IEntity
        //    where U : class, IOwnerRelation
        //{
        //    var grouped = query.GroupJoin(ctx.Set<U>(), k => k.Id, k1 => k1.EntityId, (k, k1) => new RelationEntity<T, U> { Entity = k, Relations = k1.ToList() });
        //    var groupedList = await grouped.ToListAsync();
        //    var groupedFirst = groupedList.FirstOrDefault();

        //    if (groupedFirst == null)
        //        return default(RelationEntity<T, U>);

        //    groupedFirst.Entity.RelationalOwners = groupedFirst.Relations;
        //    return groupedFirst;
        //}

        public class RelationEntity<T, U>
            where T : IHaveOwnership<U>, IEntity
            where U : IOwnerRelation
        {
            public T Entity { get; set; }
            public ICollection<U> Relations { get; set; }
        }

        public class RelationEntityPair<T, U>
            where T : IHaveOwnership<U>, IEntity
            where U : IOwnerRelation
        {
            public T Entity { get; set; }
            public U Relation { get; set; }
        }

        public static IQueryable<RelationEntityPair<T, U>> IncludeSolutionFilter<T, U>(this IQueryable<RelationEntityPair<T, U>> relationEntities, List<Guid> filter)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            return relationEntities.Where(k => !filterContainsData || filter.Contains(k.Relation.SolutionId) || k.Relation == null);

        }

        public static async Task<List<T>> ToListEntityFilteredWithSolutionAsync<T, U>(this IQueryable<RelationEntityPair<T, U>> relationEntities, List<Guid> filter)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            var list = await relationEntities.Where(k => !filterContainsData || filter.Contains(k.Relation.SolutionId) || k.Relation == null).ToListAsync();
            var groupedList = list.GroupBy(k => k.Entity, k => k.Relation, (key, g) => new RelationEntity<T, U> { Entity = key, Relations = g.ToList() }).ToList();

            relationEntities.Select(k => k.Relation).ToList();


            groupedList.ForEach(k => k.Entity.RelationalOwners = relationEntities.Select(k1 => k1.Relation).Where(k2 => k2.EntityId == k.Entity.Id).ToList());
            return groupedList.Select(k => k.Entity).ToList();
        }

        public static async Task<List<T>> ToListEntityWithSolutionAsync<T, U>(this IQueryable<RelationEntityPair<T, U>> relationEntities)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            //var groupedList = await relationEntities.GroupBy(k => k.Entity, k => k.Relation, (key, g) => new RelationEntity<T, U> { Entity = key, Relations = g.ToList() }).ToListAsync();

            //groupedList.ForEach(k => k.Entity.RelationalOwners = k.Relations);
            return await relationEntities.Select(k => k.Entity).ToListAsync();
        }

        public static async Task<T> FirstOrDefaultEntityFilteredWithSolutionAsync<T, U>(this IQueryable<RelationEntityPair<T, U>> relationEntities, List<Guid> filter)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            var list = await relationEntities.Where(k => !filterContainsData || filter.Contains(k.Relation.SolutionId) || k.Relation == null).ToListAsync();
            var groupedList = list.GroupBy(k => k.Entity, k => k.Relation, (key, g) => new RelationEntity<T, U> { Entity = key, Relations = g.ToList() }).FirstOrDefault();


            if (groupedList != null)
            {
                groupedList.Entity.RelationalOwners = await relationEntities.Select(k => k.Relation).ToListAsync();
                return groupedList.Entity;
            }
            else
                return default(T);
        }


        public static IQueryable<RelationEntityPair<T, U>> IncludeOwnershipFilter<T, U>(this IQueryable<RelationEntityPair<T, U>> relationEntities, List<PermissionDetailedDto> roleDetails)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            if (roleDetails == null)
                return relationEntities;

            List<Guid> orgs = new List<Guid>();

            roleDetails.ForEach(k => { if(k.Policies != null) orgs.AddRange(k.Policies); });
            orgs = orgs.Distinct().ToList();
            foreach (var rp in roleDetails)
            {
                if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.OnlyPolicyItself || rp.PrivilegeLevelType == PermissionGroupImpactLevel.PolicyItselfAndItsChildPolicies || rp.PrivilegeLevelType == PermissionGroupImpactLevel.AllPoliciesIncludedInZone)
                {
                    return relationEntities.Where(k => (orgs.Contains(k.Entity.OrganizationId) && (k.Entity.OwnerType != OwnerType.Role)) || (k.Entity.OwnerType == OwnerType.Role && k.Entity.OwnerId == rp.RoleId) || (k.Entity.OrganizationId == Guid.Empty) || (k.Entity.OwnerType == OwnerType.CustomerBased));
                }
                else if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.User)
                {
                    return relationEntities.Where(k => k.Entity.OwnerType == OwnerType.User && k.Entity.OwnerId == rp.UserId || (k.Entity.OwnerType == OwnerType.Role && k.Entity.OwnerId == rp.RoleId) || (k.Entity.OwnerType == OwnerType.CustomerBased));
                }
            }
            return relationEntities;
        }

        public static IQueryable<T> IncludeOwnershipFilter<T, U>(this IQueryable<T> relationEntities, List<PermissionDetailedDto> roleDetails)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            if (roleDetails == null)
                return relationEntities;

            List<Guid> orgs = new List<Guid>();
            roleDetails.ForEach(k => { if (k.Policies != null) orgs.AddRange(k.Policies); });
            orgs = orgs.Distinct().ToList();
            foreach (var rp in roleDetails)
            {
                if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.OnlyPolicyItself || rp.PrivilegeLevelType == PermissionGroupImpactLevel.PolicyItselfAndItsChildPolicies || rp.PrivilegeLevelType == PermissionGroupImpactLevel.AllPoliciesIncludedInZone)
                {
                    return relationEntities.Where(k => (orgs.Contains(k.OrganizationId) && (k.OwnerType != OwnerType.Role)) || (k.OwnerType == OwnerType.Role && k.OwnerId == rp.RoleId) || (k.OrganizationId == Guid.Empty) || (k.OwnerType == OwnerType.CustomerBased));
                }
                else if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.User)
                {
                    return relationEntities.Where(k => k.OwnerType == OwnerType.User && k.OwnerId == rp.UserId || (k.OwnerType == OwnerType.Role && k.OwnerId == rp.RoleId) || (k.OwnerType == OwnerType.CustomerBased));
                }
            }
            return relationEntities;
        }
    }
}
