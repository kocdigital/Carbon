
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

    public class CarbonContext<TContext> : DbContext where TContext : DbContext
    {
        public CarbonContext(DbContextOptions options) : base(options)
        {

        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.CurrentValues["IsDeleted"] = true;
                    entry.State = EntityState.Modified;
                    CascadeSoftDelete(entry.Navigations.ToList());
                }
            }

            foreach (var entry in ChangeTracker.Entries<IDeleteAuditing>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.CurrentValues["DeletedDate"] = DateTime.UtcNow;
                }
            }

            foreach (var entry in ChangeTracker.Entries<IInsertAuditing>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.CurrentValues["InsertedDate"] = DateTime.UtcNow;
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
