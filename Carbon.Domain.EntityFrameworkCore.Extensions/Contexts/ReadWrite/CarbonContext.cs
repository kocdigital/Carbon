using Carbon.Domain.Abstractions.Attributes;
using Carbon.Domain.Abstractions.Entities;
using Carbon.Domain.EntityFrameworkCore.CentralizedAudit;
using EFCoreAuditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.Domain.EntityFrameworkCore
{
    /// <summary>
    ///     A carbon wrapper class for database context objects.
    /// </summary>
    /// <typeparam name="TContext"> A database context object to be wrapped. </typeparam>
    public class CarbonContext<TContext> : AuditLogDbContext<Guid> where TContext : DbContext
    {
        private Dictionary<EntityState, List<(string AttributeName, Action<EntityEntry> Action)>> navigationOps;

        /// <summary>
        ///     Constructor that initializes the CarbonContext with the given options for database context
        /// </summary>
        /// <param name="options"> Options for DbContext constructor </param>
        public CarbonContext(DbContextOptions options) : base(options)
        {
            navigationOps = new Dictionary<EntityState, List<(string, Action<EntityEntry>)>>();
            navigationOps[EntityState.Deleted] = new List<(string AttributeName, Action<EntityEntry> Action)>
            {
				//Order of items makes sense! Be careful when changing!
				(nameof(DoCascadeDelete), SoftDelete),
                (nameof(SoftDeleteConstraint), CheckSoftDeleteConstraint)
            };
        }

        /// <summary>
        ///     Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Set to true to discard tracked changes after the save operation ends in success.</param>
        /// <returns>Number of state entries written to the database.</returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        ///     Saves all changes made in this context to the database.
        /// </summary>
        /// <remarks> 
        ///     Multiple SaveChangesAsync operations on the same context is not supported, therefore it is advised to call
        ///     use the "await" keyword to let the task finish before any other operations on this context.
        /// </remarks>
        /// <param name="acceptAllChangesOnSuccess">Set to true to discard tracked changes after the save operation ends in success.</param>
        /// <returns>A task with number of state entries written to the database as its result.</returns>

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Checks property is exist or not and sets datetimenowutc 
        /// </summary>
        /// <param name="propertyValues">property values</param>
        /// <param name="name">property name</param>
        private void SetDateTimeToProperty(PropertyValues propertyValues, string name)
        {
            if (propertyValues?.Properties != null && propertyValues.Properties.Any(x => x.Name == name))
            {
                propertyValues[name] = DateTime.UtcNow;
            }
        }

        /// <summary>
        ///     Adds necessary information to changed items in the context before they can be properly saved to the database.
        /// </summary>
        /// <remarks>
        ///     Automatically gets called when SaveChanges or SaveChangesAsync is called.
        /// </remarks>
        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.CurrentValues["IsDeleted"] = true;
                    SetDateTimeToProperty(entry.CurrentValues, "DeletedDate");
                    SetDateTimeToProperty(entry.CurrentValues, "UpdatedDate");
                    entry.State = EntityState.Modified;
                    NavigationVisitor(entry.Navigations.ToList(), EntityState.Deleted);
                }
            }

            foreach (var entry in ChangeTracker.Entries<IDeleteAuditing>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    var obj = entry.CurrentValues;
                    entry.CurrentValues["DeletedDate"] = DateTime.UtcNow;
                    SetDateTimeToProperty(entry.CurrentValues, "UpdatedDate");
                }
            }

            foreach (var entry in ChangeTracker.Entries<IInsertAuditing>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.CurrentValues["InsertedDate"] = DateTime.UtcNow;
                    SetDateTimeToProperty(entry.CurrentValues, "UpdatedDate");
                }
            }

            foreach (var entry in ChangeTracker.Entries<IUpdateAuditing>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.CurrentValues["UpdatedDate"] = DateTime.UtcNow;
                }
            }

        }

        /// <summary>
        ///     Checks the relation; if related entity is not deleted or soft deleted, throws <see cref="InvalidOperationException"/>
        /// </summary>
        /// <remarks>
        ///     Automatically gets called when SaveChanges or SaveChangesAsync is called.
        /// </remarks>
        /// <param name="relatedEntry">Related entry to be operated on. </param>
        private void CheckSoftDeleteConstraint(EntityEntry relatedEntry)
        {
            if (relatedEntry == null)
                return;

            if (!((typeof(ISoftDelete).IsAssignableFrom(relatedEntry.Entity.GetType()) & (bool)relatedEntry.CurrentValues["IsDeleted"] == true)
                | relatedEntry.State == EntityState.Deleted))
                throw new InvalidOperationException($"Could not continue delete operation! There is a related entity which is not deleted! Relation Entity: {relatedEntry.Metadata.DisplayName()}");
        }

        /// <summary>
        ///     Updates database context entry to perform soft delete before entry can be saved to the database.
        /// </summary>
        /// <remarks>
        ///     Automatically gets called when SaveChanges or SaveChangesAsync is called.
        /// </remarks>
        /// <param name="relatedEntry">Related entry to be operated on. </param>
        private void SoftDelete(EntityEntry relatedEntry)
        {
            if (relatedEntry == null)
                return;
            if (typeof(ISoftDelete).IsAssignableFrom(relatedEntry.Entity.GetType()))
            {
                relatedEntry.CurrentValues["IsDeleted"] = true;
                SetDateTimeToProperty(relatedEntry.CurrentValues, "DeletedDate");
                SetDateTimeToProperty(relatedEntry.CurrentValues, "UpdatedDate");
                relatedEntry.State = EntityState.Modified;
            }
            else
            {
                relatedEntry.State = EntityState.Deleted;
            }
        }

        /// <summary>
        ///     Recursively updates database context entries to perform defined operations before entries can be saved to the database.
        /// </summary>
        /// <remarks>
        ///     Automatically gets called when SaveChanges or SaveChangesAsync is called.
        /// </remarks>
        /// <param name="entries"> List of entries to be operated on. </param>
        /// <param name="visitingOperation">Type of current operation</param>
        private void NavigationVisitor(IEnumerable<NavigationEntry> entries, EntityState visitingOperation)
        {
            if (entries.Count() == 0 || !navigationOps.ContainsKey(visitingOperation))
                return;

            foreach (var navItem in entries)
            {
                var attributes = navigationOps[visitingOperation].Select(x => x.AttributeName).Where(x => navItem.Metadata.PropertyInfo.CustomAttributes.Any(y => y.AttributeType.Name == x));
                if (attributes.Any())
                {
                    if (navItem is CollectionEntry collectionEntry)
                    {
                        if (collectionEntry?.CurrentValue == null && !collectionEntry.IsLoaded)
                            collectionEntry.Load();
                        if (collectionEntry?.CurrentValue != null)
                        {
                            foreach (var dependentEntry in collectionEntry.CurrentValue)
                            {
                                var relatedEntry = Entry(dependentEntry);
                                foreach (var navigationActions in navigationOps[visitingOperation])
                                {
                                    if (attributes.Contains(navigationActions.AttributeName))
                                    {
                                        navigationActions.Action.Invoke(relatedEntry);
                                    }
                                }

                                NavigationVisitor(relatedEntry.Navigations.ToList(), visitingOperation);
                            }
                        }
                    }
                    else
                    {
                        if (navItem?.CurrentValue == null && !navItem.IsLoaded)
                            navItem.Load();
                        var dependentEntry = navItem.CurrentValue;

                        if (dependentEntry != null)
                        {
                            var relatedEntry = Entry(dependentEntry);
                            foreach (var navigationActions in navigationOps[visitingOperation])
                            {
                                if (attributes.Contains(navigationActions.AttributeName))
                                {
                                    navigationActions.Action.Invoke(relatedEntry);
                                }
                            }

                            NavigationVisitor(relatedEntry.Navigations.ToList(), visitingOperation);
                        }
                    }
                }
            }
        }
    }
}
