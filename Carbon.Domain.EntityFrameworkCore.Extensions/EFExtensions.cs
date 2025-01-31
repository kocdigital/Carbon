﻿using Carbon.Common;
using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.Domain.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Carbon.Common.Extensions;
using Carbon.PagedList;
using System.Reflection;

namespace Carbon.Domain.EntityFrameworkCore
{
    /// <summary>
    /// Contains extension methods like IncludeSolutionFilter, ToListEntityFilteredWithSolutionAsync etc.
    /// </summary>
    public static class EFExtensions
    {
        /// <summary>
        /// Executes a StoredProcedure in MSSQL and Function in PostgreSQL, generally used for procedures that returns a table result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">DbContext Inherited Context</param>
        /// <param name="procedureName">Your Stored Procedure or Function Name</param>
        /// <param name="parameters">Your parameters respectively as in the procedure (Same Order)</param>
        /// <returns></returns>
        public static IQueryable<T> ExecuteProcedureSql<T>(this DbContext context, string procedureName,
            params object[] parameters)
            where T : class
        {
            List<String> sb = new List<String>();
            foreach (var param in parameters)
            {
                if (param.GetType() == typeof(Guid))
                    sb.Add($"'{param.ToString()}'");
                else if (param.GetType() == typeof(bool) || param.GetType() == typeof(Int32) ||
                         param.GetType() == typeof(long) || param.GetType() == typeof(Int16))
                    sb.Add($"{param}");
                else if (param.GetType() == typeof(DateTime))
                    sb.Add($"'{((DateTime)param).ToString("YYYY-MM-dd HH:mm:ss")}'");
                else
                    sb.Add($"'{param.ToString()}'");
            }

            string sqlQuery = null;
            if (context.Database.IsNpgsql())
            {
                sqlQuery = $"select * from {procedureName} ({String.Join(',', sb)});";
            }
            else if (context.Database.IsSqlServer())
            {
                sqlQuery = $"EXEC {procedureName} {String.Join(',', sb)}";
            }
            else
            {
                throw new NotSupportedException($"{context.Database.ProviderName} not supported!");
            }

            Console.WriteLine("Execute Procedure Sql: " + sqlQuery);
            return context.Set<T>().FromSqlRaw($"{sqlQuery}");

        }

        public static async Task<ICollection<T>> ToEntityListIncludeOwnershipAsync<T, U>(this IQueryable<T> query,
            DbContext ctx)
            where T : IHaveOwnership<U>, IEntity
            where U : class, IOwnerRelation
        {
            var grouped = query.GroupJoin(ctx.Set<U>(), k => k.Id, k1 => k1.EntityId,
                (k, k1) => new RelationEntity<T, U> { Entity = k, Relations = k1.ToList() });
            var groupedList = await grouped.ToListAsync();
            groupedList.ForEach(k => k.Entity.RelationalOwners = k.Relations);
            return groupedList.Select(k => k.Entity).ToList();
        }

        public static ICollection<T> ToEntityListIncludeOwnershipAsync<T, U>(this IEnumerable<T> query, DbContext ctx)
            where T : IHaveOwnership<U>, IEntity
            where U : class, IOwnerRelation
        {
            var grouped = query.GroupJoin(ctx.Set<U>(), k => k.Id, k1 => k1.EntityId,
                (k, k1) => new RelationEntity<T, U> { Entity = k, Relations = k1.ToList() });
            var groupedList = grouped.ToList();
            groupedList.ForEach(k => k.Entity.RelationalOwners = k.Relations);
            return groupedList.Select(k => k.Entity).ToList();
        }

        public static IQueryable<RelationEntityPair<T, U>> IncludeSolutionRelation<T, U>(this IQueryable<T> query,
            DbContext ctx)
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

        public static IEnumerable<RelationEntityPair<T, U>> IncludeSolutionRelation<T, U>(this IEnumerable<T> query,
            DbContext ctx)
            where T : IHaveOwnership<U>, IEntity
            where U : class, IOwnerRelation, ISoftDelete
        {
            var daList = (from q in query
                          join io in ctx.Set<U>().Where(k => !k.IsDeleted) on q.Id equals io.EntityId
                              into ljoin
                          from lj in ljoin.DefaultIfEmpty()
                          select new RelationEntityPair<T, U> { Entity = q, Relation = lj });

            return daList;
        }

        public static async Task<T> EntityFirstOrDefaultIncludeOwnershipAsync<T, U>(this IQueryable<T> query,
            DbContext ctx)
            where T : IHaveOwnership<U>, IEntity
            where U : class, IOwnerRelation
        {
            var grouped = query.GroupJoin(ctx.Set<U>(), k => k.Id, k1 => k1.EntityId,
                (k, k1) => new RelationEntity<T, U> { Entity = k, Relations = k1.ToList() });

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

        public static IQueryable<RelationEntityPair<T, U>> IncludeSolutionFilter<T, U>(
            this IQueryable<RelationEntityPair<T, U>> relationEntities, List<Guid> filter)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            return relationEntities.Where(k =>
                !filterContainsData || k.Relation == null || filter.Contains(k.Relation.SolutionId));

        }

        public static IEnumerable<RelationEntityPair<T, U>> IncludeSolutionFilter<T, U>(
            this IEnumerable<RelationEntityPair<T, U>> relationEntities, List<Guid> filter)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            return relationEntities.Where(k =>
                !filterContainsData || k.Relation == null || filter.Contains(k.Relation.SolutionId));

        }


        public static async Task<List<T>> ToListEntityFilteredWithSolutionAsync<T, U>(
            this IQueryable<RelationEntityPair<T, U>> relationEntities, List<Guid> filter)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            var list = await relationEntities
                .Where(k => !filterContainsData || k.Relation == null || filter.Contains(k.Relation.SolutionId))
                .ToListAsync();
            var groupedList = list.GroupBy(k => k.Entity, k => k.Relation,
                (key, g) => new RelationEntity<T, U> { Entity = key, Relations = g.ToList() }).ToList();

            var relationEntitiesAsList =
                relationEntities.Where(k => k.Relation != null).Select(k => k.Relation).ToList();


            groupedList.ForEach(k =>
                k.Entity.RelationalOwners = relationEntitiesAsList.Where(k2 => k2 != null && k2.EntityId == k.Entity.Id)
                    .ToList());
            return groupedList.Select(k => k.Entity).ToList();
        }

        public static List<T> ToListEntityFilteredWithSolutionAsync<T, U>(
            this IEnumerable<RelationEntityPair<T, U>> relationEntities, List<Guid> filter)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            var list = relationEntities.Where(k =>
                !filterContainsData || k.Relation == null || filter.Contains(k.Relation.SolutionId)).ToList();
            var groupedList = list.GroupBy(k => k.Entity, k => k.Relation,
                (key, g) => new RelationEntity<T, U> { Entity = key, Relations = g.ToList() }).ToList();

            var relationEntitiesAsList =
                relationEntities.Where(k => k.Relation != null).Select(k => k.Relation).ToList();

            groupedList.ForEach(k =>
                k.Entity.RelationalOwners = relationEntitiesAsList.Where(k2 => k2 != null && k2.EntityId == k.Entity.Id)
                    .ToList());

            return groupedList.Select(k => k.Entity).ToList();
        }

        public static async Task<List<T>> ToListEntityWithSolutionAsync<T, U>(
            this IQueryable<RelationEntityPair<T, U>> relationEntities)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            //var groupedList = await relationEntities.GroupBy(k => k.Entity, k => k.Relation, (key, g) => new RelationEntity<T, U> { Entity = key, Relations = g.ToList() }).ToListAsync();

            //groupedList.ForEach(k => k.Entity.RelationalOwners = k.Relations);
            return await relationEntities.Select(k => k.Entity).ToListAsync();
        }

        /// <summary>
        /// Retrieves a paged list of entities with filtered relations based on specified criteria and ordering.
        /// In this method, data is filtered according to ownership information.
        /// </summary>
        /// <typeparam name="T">The type of entities that have ownership relationships with type U and implement IEntity and IHaveOwnership interfaces.</typeparam>
        /// <typeparam name="U">The type of entity relations that are associated with type T and implement EntitySolutionRelation interface.</typeparam>
        /// <param name="relationEntities">The IQueryable of relation entities containing pairs of entities of type T and relations of type U.</param>
        /// <param name="filter">A list of Guids used to filter the relations based on SolutionId, can be null or empty to include all relations.</param>
        /// <param name="orderables">A list of Orderable objects specifying the ordering of the entities.</param>
        /// <param name="pageIndex">The index of the page to retrieve (starting from 0).</param>
        /// <param name="pageSize">The maximum number of entities per page.</param>
        /// <returns>A PagedList containing the requested page of entities with filtered relations, based on the specified criteria.</returns>
        /// <remarks>
        /// This method retrieves a paged list of entities of type T that have ownership relationships with type U.
        /// The method filters the relations based on the provided list of Guids, which represent SolutionIds, if the filter list is not null or empty.
        /// The resulting entities are ordered based on the specified ordering criteria and returned as a paged list.
        /// </remarks>
        public static PagedList<T> ToPagedListEntityFilteredWithSolution<T, U>(
            this IQueryable<EFExtensions.RelationEntityPair<T, U>> relationEntities,
            List<Guid> filter,
            IList<Orderable> orderables,
            int pageIndex,
            int pageSize)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            var query = relationEntities
                .Where(k => !filterContainsData || k.Relation == null || filter.Contains(k.Relation.SolutionId))
                .Select(k => k.Entity)
                .Distinct();

            var filteredQuery = query.OrderBy(orderables);
            if (pageSize != 0)
            {
                filteredQuery = filteredQuery.SkipTake(pageIndex, pageSize);
            }

            var entities = filteredQuery.ToList();

            var relationEntitiesAsList =
                relationEntities.Where(k => k.Relation != null).Select(k => k.Relation).ToList();

            entities.ForEach(entity =>
            {
                entity.RelationalOwners = relationEntitiesAsList
                    .Where(rel => rel != null && rel.EntityId == entity.Id).ToList();
            });

            var totalDataCount = query.Count();
            var result = new PagedList<T>(entities, pageIndex, pageSize, totalDataCount);
            return result;
        }

        /// <summary>
        /// Retrieves a paged list of entities with filtered relations based on specified criteria and ordering.
        /// In this method, data is filtered according to ownership information.
        /// </summary>
        /// <typeparam name="T">The type of entities that have ownership relationships with type U and implement IEntity and IHaveOwnership interfaces.</typeparam>
        /// <typeparam name="U">The type of entity relations that are associated with type T and implement EntitySolutionRelation interface.</typeparam>
        /// <param name="relationEntities">The IQueryable of relation entities containing pairs of entities of type T and relations of type U.</param>
        /// <param name="filter">A list of Guids used to filter the relations based on SolutionId, can be null or empty to include all relations.</param>
        /// <param name="orderables">A list of Orderable objects specifying the ordering of the entities.</param>
        /// <param name="pageIndex">The index of the page to retrieve (starting from 0).</param>
        /// <param name="pageSize">The maximum number of entities per page.</param>
        /// <returns>A PagedList containing the requested page of entities with filtered relations, based on the specified criteria.</returns>
        /// <remarks>
        /// This method retrieves a paged list of entities of type T that have ownership relationships with type U.
        /// The method filters the relations based on the provided list of Guids, which represent SolutionIds, if the filter list is not null or empty.
        /// The resulting entities are ordered based on the specified ordering criteria and returned as a paged list.
        /// </remarks>
        public static async Task<PagedList<T>> ToPagedListEntityFilteredWithSolutionAsync<T, U>(
            this IQueryable<EFExtensions.RelationEntityPair<T, U>> relationEntities,
            List<Guid> filter,
            IList<Orderable> orderables,
            int pageIndex,
            int pageSize)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            var query = relationEntities
                .Where(k => !filterContainsData || k.Relation == null || filter.Contains(k.Relation.SolutionId))
                .Select(k => k.Entity)
                .Distinct();

            var filteredQuery = query.OrderBy(orderables);
            if (pageSize != 0)
            {
                filteredQuery = filteredQuery.SkipTake(pageIndex, pageSize);
            }

            var entities = await filteredQuery.ToListAsync();

            var relationEntitiesAsList =
                relationEntities.Where(k => k.Relation != null).Select(k => k.Relation).ToList();

            entities.ForEach(entity =>
            {
                entity.RelationalOwners = relationEntitiesAsList
                    .Where(rel => rel != null && rel.EntityId == entity.Id).ToList();
            });

            var totalDataCount = await query.CountAsync();
            var result = new PagedList<T>(entities, pageIndex, pageSize, totalDataCount);
            return result;
        }
        public static async Task<T> FirstOrDefaultEntityFilteredWithSolutionAsync<T, U>(
            this IQueryable<RelationEntityPair<T, U>> relationEntities, List<Guid> filter)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            var list = await relationEntities
                .Where(k => !filterContainsData || k.Relation == null || filter.Contains(k.Relation.SolutionId))
                .ToListAsync();
            var groupedList = list.GroupBy(k => k.Entity, k => k.Relation,
                (key, g) => new RelationEntity<T, U> { Entity = key, Relations = g.ToList() }).FirstOrDefault();


            if (groupedList != null)
            {
                groupedList.Entity.RelationalOwners = await relationEntities.Where(k => k.Relation != null)
                    .Select(k => k.Relation).ToListAsync();
                return groupedList.Entity;
            }
            else
                return default(T);
        }

        public static T FirstOrDefaultEntityFilteredWithSolutionAsync<T, U>(
            this IEnumerable<RelationEntityPair<T, U>> relationEntities, List<Guid> filter)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            var filterContainsData = filter != null && filter.Any();

            var list = relationEntities.Where(k =>
                !filterContainsData || k.Relation == null || filter.Contains(k.Relation.SolutionId)).ToList();
            var groupedList = list.GroupBy(k => k.Entity, k => k.Relation,
                (key, g) => new RelationEntity<T, U> { Entity = key, Relations = g.ToList() }).FirstOrDefault();


            if (groupedList != null)
            {
                groupedList.Entity.RelationalOwners =
                    relationEntities.Where(k => k.Relation != null).Select(k => k.Relation).ToList();
                return groupedList.Entity;
            }
            else
                return default(T);
        }


        public static IQueryable<RelationEntityPair<T, U>> IncludeOwnershipFilter<T, U>(
            this IQueryable<RelationEntityPair<T, U>> relationEntities, List<PermissionDetailedDto> roleDetails)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            if (roleDetails == null)
                return relationEntities;

            List<Guid> orgs = new List<Guid>();

            roleDetails.ForEach(k =>
            {
                if (k.Policies != null) orgs.AddRange(k.Policies);
            });
            orgs = orgs.Distinct().ToList();
            foreach (var rp in roleDetails)
            {
                if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.OnlyPolicyItself ||
                    rp.PrivilegeLevelType == PermissionGroupImpactLevel.PolicyItselfAndItsChildPolicies ||
                    rp.PrivilegeLevelType == PermissionGroupImpactLevel.AllPoliciesIncludedInZone)
                {
                    return relationEntities.Where(k =>
                        (orgs.Contains(k.Entity.OrganizationId) && (k.Entity.OwnerType != OwnerType.Role)) ||
                        (k.Entity.OwnerType == OwnerType.Role && k.Entity.OwnerId == rp.RoleId) ||
                        (k.Entity.OrganizationId == Guid.Empty) || k.Entity.OwnerType == OwnerType.CustomerBased ||
                        k.Entity.OwnerType == OwnerType.None);
                }
                else if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.User)
                {
                    return relationEntities.Where(k =>
                        k.Entity.OwnerType == OwnerType.User && k.Entity.OwnerId == rp.UserId ||
                        (k.Entity.OwnerType == OwnerType.Role && k.Entity.OwnerId == rp.RoleId) ||
                        k.Entity.OwnerType == OwnerType.CustomerBased || k.Entity.OwnerType == OwnerType.None);
                }
            }

            return relationEntities;
        }

        public static IEnumerable<RelationEntityPair<T, U>> IncludeOwnershipFilter<T, U>(
            this IEnumerable<RelationEntityPair<T, U>> relationEntities, List<PermissionDetailedDto> roleDetails)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            if (roleDetails == null)
                return relationEntities;

            List<Guid> orgs = new List<Guid>();

            roleDetails.ForEach(k =>
            {
                if (k.Policies != null) orgs.AddRange(k.Policies);
            });
            orgs = orgs.Distinct().ToList();
            foreach (var rp in roleDetails)
            {
                if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.OnlyPolicyItself ||
                    rp.PrivilegeLevelType == PermissionGroupImpactLevel.PolicyItselfAndItsChildPolicies ||
                    rp.PrivilegeLevelType == PermissionGroupImpactLevel.AllPoliciesIncludedInZone)
                {
                    return relationEntities.Where(k =>
                        (orgs.Contains(k.Entity.OrganizationId) && (k.Entity.OwnerType != OwnerType.Role)) ||
                        (k.Entity.OwnerType == OwnerType.Role && k.Entity.OwnerId == rp.RoleId) ||
                        (k.Entity.OrganizationId == Guid.Empty) || k.Entity.OwnerType == OwnerType.CustomerBased ||
                        k.Entity.OwnerType == OwnerType.None);
                }
                else if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.User)
                {
                    return relationEntities.Where(k =>
                        k.Entity.OwnerType == OwnerType.User && k.Entity.OwnerId == rp.UserId ||
                        (k.Entity.OwnerType == OwnerType.Role && k.Entity.OwnerId == rp.RoleId) ||
                        k.Entity.OwnerType == OwnerType.CustomerBased || k.Entity.OwnerType == OwnerType.None);
                }
            }

            return relationEntities;
        }

        public static IQueryable<T> IncludeOwnershipFilter<T, U>(this IQueryable<T> relationEntities,
            List<PermissionDetailedDto> roleDetails)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            if (roleDetails == null)
                return relationEntities;

            List<Guid> orgs = new List<Guid>();
            roleDetails.ForEach(k =>
            {
                if (k.Policies != null) orgs.AddRange(k.Policies);
            });
            orgs = orgs.Distinct().ToList();
            foreach (var rp in roleDetails)
            {
                if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.OnlyPolicyItself ||
                    rp.PrivilegeLevelType == PermissionGroupImpactLevel.PolicyItselfAndItsChildPolicies ||
                    rp.PrivilegeLevelType == PermissionGroupImpactLevel.AllPoliciesIncludedInZone)
                {
                    return relationEntities.Where(k =>
                        (orgs.Contains(k.OrganizationId) && (k.OwnerType != OwnerType.Role)) ||
                        (k.OwnerType == OwnerType.Role && k.OwnerId == rp.RoleId) || (k.OrganizationId == Guid.Empty) ||
                        k.OwnerType == OwnerType.CustomerBased || k.OwnerType == OwnerType.None);
                }
                else if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.User)
                {
                    return relationEntities.Where(k =>
                        k.OwnerType == OwnerType.User && k.OwnerId == rp.UserId ||
                        (k.OwnerType == OwnerType.Role && k.OwnerId == rp.RoleId) ||
                        k.OwnerType == OwnerType.CustomerBased || k.OwnerType == OwnerType.None);
                }
            }

            return relationEntities;
        }

        public static IEnumerable<T> IncludeOwnershipFilter<T, U>(this IEnumerable<T> relationEntities,
            List<PermissionDetailedDto> roleDetails)
            where T : IHaveOwnership<U>, IEntity
            where U : EntitySolutionRelation
        {
            if (roleDetails == null)
                return relationEntities;

            List<Guid> orgs = new List<Guid>();
            roleDetails.ForEach(k =>
            {
                if (k.Policies != null) orgs.AddRange(k.Policies);
            });
            orgs = orgs.Distinct().ToList();
            foreach (var rp in roleDetails)
            {
                if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.OnlyPolicyItself ||
                    rp.PrivilegeLevelType == PermissionGroupImpactLevel.PolicyItselfAndItsChildPolicies ||
                    rp.PrivilegeLevelType == PermissionGroupImpactLevel.AllPoliciesIncludedInZone)
                {
                    return relationEntities.Where(k =>
                        (orgs.Contains(k.OrganizationId) && (k.OwnerType != OwnerType.Role)) ||
                        (k.OwnerType == OwnerType.Role && k.OwnerId == rp.RoleId) || (k.OrganizationId == Guid.Empty) ||
                        k.OwnerType == OwnerType.CustomerBased || k.OwnerType == OwnerType.None);
                }
                else if (rp.PrivilegeLevelType == PermissionGroupImpactLevel.User)
                {
                    return relationEntities.Where(k =>
                        k.OwnerType == OwnerType.User && k.OwnerId == rp.UserId ||
                        (k.OwnerType == OwnerType.Role && k.OwnerId == rp.RoleId) ||
                        k.OwnerType == OwnerType.CustomerBased || k.OwnerType == OwnerType.None);
                }
            }

            return relationEntities;
        }

        /// <summary>
        /// Converts an IQueryable of entities into a paged list asynchronously based on the specified ordering, page index, and page size.
        /// </summary>
        /// <typeparam name="T">The type of entities in the IQueryable, must implement IEntity interface.</typeparam>
        /// <param name="query">The IQueryable of entities to be paged.</param>
        /// <param name="orderables">A list of Orderable objects specifying the ordering of the entities.</param>
        /// <param name="pageIndex">The index of the page to retrieve (starting from 0).</param>
        /// <param name="pageSize">The maximum number of entities per page.</param>
        /// <returns>A Task representing the asynchronous operation that returns a PagedList containing the requested page of entities.</returns>
        public static async Task<PagedList<T>> ToPagedListEntityAsync<T>(
            this IQueryable<T> query,
            IList<Orderable> orderables,
            int pageIndex,
            int pageSize)
            where T : IEntity
        {
            var filteredQuery = query.AsQueryable().OrderBy(orderables);
            if (pageSize != 0)
            {
                filteredQuery = filteredQuery.SkipTake(pageIndex, pageSize);
            }

            var data = await filteredQuery.ToListAsync();
            var totalDataCount = await query.CountAsync();

            var result = new PagedList<T>(data, pageIndex, pageSize, totalDataCount);
            return result;
        }

        /// <summary>
        /// Filters an IQueryable based on whether any word in the search term exists in the specified property's string value, using a case-insensitive comparison.
        /// </summary>
        /// <typeparam name="T">The type of elements in the IQueryable.</typeparam>
        /// <param name="query">The IQueryable to filter.</param>
        /// <param name="selector">Expression specifying the property to search within.</param>
        /// <param name="search">The search term containing one or more words.</param>
        /// <param name="searchByWords">A boolean value indicating whether to search for individual words in the search term (default is false).</param>
        /// <returns>The filtered IQueryable containing elements where any word in the search term exists in the specified property's string value.</returns>
        /// <remarks>
        /// This method performs a case-insensitive search. If searchByWords is false, it performs a normal contains search for the entire search term. If searchByWords is true, it splits the search term into words and checks if any word exists in the specified property's string value.
        /// </remarks>
        public static IQueryable<T> WhereContains<T>(this IQueryable<T> query, Expression<Func<T, string>> selector, string search, bool searchByWords = false, bool useTrEnSearch = true)
        {
            return WhereContains(query, selector, new List<string> { search }, searchByWords, useTrEnSearch);
        }

        /// <summary>
        /// Filters an IQueryable based on whether any word in the search term exists in the specified property's string value, using a case-insensitive comparison.
        /// </summary>
        /// <typeparam name="T">The type of elements in the IQueryable.</typeparam>
        /// <param name="query">The IQueryable to filter.</param>
        /// <param name="selector">Expression specifying the property to search within.</param>
        /// <param name="search">The search term containing one or more words.</param>
        /// <param name="searchByWords">A boolean value indicating whether to search for individual words in the search term (default is false).</param>
        /// <param name="useTrEnSearch">A boolean value if is TRUE then generates Contains expression or if is FALSE then generates Equals expression (default is true)</param>
        /// <returns>The filtered IQueryable containing elements where any word in the search term exists in the specified property's string value.</returns>
        /// <remarks>
        /// This method performs a case-insensitive search. If searchByWords is false, it performs a normal contains search for the entire search term. If searchByWords is true, it splits the search term into words and checks if any word exists in the specified property's string value.
        /// </remarks>

        public static IQueryable<T> WhereContains<T>(
            this IQueryable<T> query,
            Expression<Func<T, string>> selector,
            List<string> search,
            bool searchByWords = false,
            bool useTrEnSearch = true)
        {
            if (query == null || selector == null || !search.Any())
            {
                return query;
            }

            CultureInfo culture = new CultureInfo("tr-TR", useUserOverride: false);
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
            InvocationExpression instance = Expression.Invoke(selector, parameterExpression);
            MethodInfo method = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            MethodCallExpression instance2 = Expression.Call(instance, method);
            Expression expression = null;

            foreach (string value in search)
            {
                if (searchByWords)
                {
                    var splittedText = value.Split(" ");

                    foreach (var splittedTextItem in splittedText)
                    {
                        ConstantExpression constantExpression = Expression.Constant(splittedTextItem.ToLower(culture));
                        MethodCallExpression methodCallExpression = Expression.Call(instance2, "Contains", Type.EmptyTypes, constantExpression);
                        expression = ((expression != null) ? ((Expression)Expression.AndAlso(expression, methodCallExpression)) : ((Expression)methodCallExpression));
                    }
                }
                else
                {
                    if (useTrEnSearch)
                    {
                        ConstantExpression constantExpression = Expression.Constant(value.ToLower(culture));
                        MethodCallExpression methodCallExpression = Expression.Call(instance2, "Contains", Type.EmptyTypes, constantExpression);
                        expression = ((expression != null) ? ((Expression)Expression.OrElse(expression, methodCallExpression)) : ((Expression)methodCallExpression));
                    }
                    else
                    {
                        MethodInfo methodEquals = typeof(string).GetMethod("Equals", new[] { typeof(string) });

                        ConstantExpression constantExpression = Expression.Constant(value.ToLower(culture));
                        MethodCallExpression methodCallExpression = Expression.Call(instance2, methodEquals, constantExpression);
                        expression = ((expression != null) ? ((Expression)Expression.OrElse(expression, methodCallExpression)) : ((Expression)methodCallExpression));
                    }
                }
            }

            Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[1] { parameterExpression });
            query = query.Where(predicate);
            return query;
        }


        /// <summary>
        /// Applies a predicate filter to the source sequence based on the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="query">The queryable sequence.</param>
        /// <param name="condition">The condition that determines whether the predicate should be applied.</param>
        /// <param name="predicate">The filtering predicate.</param>
        /// <returns>The initial sequence potentially filtered by the predicate based on the specified condition.</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>
        /// Applies a 'Contains' filtering operation to the source sequence based on the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="query">The queryable sequence.</param>
        /// <param name="condition">The condition that determines whether the 'Contains' filter should be applied.</param>
        /// <param name="predicate">The filtering predicate.</param>
        /// <param name="search">The filtering value to be searched within the property specified by the predicate.</param>
        /// <param name="searchByWords">Specifies whether the 'Contains' operation should be applied to each word in the property or to the property as a whole.</param>
        /// <returns>The initial sequence potentially filtered by the 'Contains' operation based on the specified condition.</returns>
        public static IQueryable<T> WhereIfContains<T>(this IQueryable<T> query, bool condition,
            Expression<Func<T, string>> predicate,
            string search,
            bool searchByWords = false)
        {
            return condition ? query.WhereContains(predicate, search, searchByWords) : query;
        }
    }
}