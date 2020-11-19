using Carbon.Domain.Abstractions.Entities;
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
    public class CarbonContext<TContext> : DbContext where TContext : DbContext
    {
        /// <summary>
        ///     Constructor that initializes the CarbonContext with the given options for database context
        /// </summary>
        /// <param name="options"> Options for DbContext constructor </param>
        public CarbonContext(DbContextOptions options) : base(options)
        {

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
            if (propertyValues?.Properties!=null && propertyValues.Properties.Any(x => x.Name == name))
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
                    CascadeSoftDelete(entry.Navigations.ToList());
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
        ///     Recursively updates database context entries to perform soft delete before entries can be saved to the database.
        /// </summary>
        /// <remarks>
        ///     Automatically gets called when SaveChanges or SaveChangesAsync is called.
        /// </remarks>
        /// <param name="entries"> List of entries to be operated on. </param>
        private void CascadeSoftDelete(IEnumerable<NavigationEntry> entries)
        {
            if (entries.Count() == 0)
                return;

            foreach (var navItem in entries)
            {
                if (navItem.Metadata.PropertyInfo.CustomAttributes.Any(x => x.AttributeType.Name == "DoCascadeDelete"))
                {
                    if (navItem is CollectionEntry collectionEntry)
                    {
                        if (collectionEntry?.CurrentValue != null)
                        {
                            foreach (var dependentEntry in collectionEntry.CurrentValue)
                            {
                                var relatedEntry = Entry(dependentEntry);

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

                                CascadeSoftDelete(relatedEntry.Navigations.ToList());
                            }
                        }
                    }
                    else
                    {
                        var dependentEntry = navItem.CurrentValue;

                        if (dependentEntry != null)
                        {
                            var relatedEntry = Entry(dependentEntry);

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

                            CascadeSoftDelete(relatedEntry.Navigations.ToList());
                        }
                    }

                }
            }
        }

    }
}
