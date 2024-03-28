﻿using Carbon.Domain.EntityFrameworkCore.CentralizedAudit.Contracts;
using Carbon.Domain.EntityFrameworkCore.CentralizedAudit.Models;
using EFCoreAuditing.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.Domain.EntityFrameworkCore.CentralizedAudit
{
    public class AuditLogDbContext<TKey> : DbContext where TKey : IEquatable<TKey>
    {
        #region Variables
        public string CurrentUserId { get; set; }
        IContextConfiguration _contextConfiguration { get; }
        public DbSet<Audit> Audits { get; set; }
        #endregion

        public AuditLogDbContext(
            DbContextOptions options,
            IContextConfiguration contextConfiguration = null)
            : base(options)
        {
            _contextConfiguration = contextConfiguration;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var entity = builder.Entity<Audit>();

            entity.Property(p => p.NewValues).HasColumnType("text");
            entity.Property(p => p.OldValues).HasColumnType("text");
            entity.Property(p => p.KeyValues).HasColumnType("text");

            base.OnModelCreating(builder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();

            SoftDelete();

            List<AuditEntry> auditEntries = OnBeforeSaveChanges();

            var result = await base.SaveChangesAsync(cancellationToken);

            await OnAfterSaveChangesAsync(auditEntries);

            return result;
        }


        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();

            var auditEntries = new List<AuditEntry>();

            foreach (EntityEntry entry in ChangeTracker.Entries().ToList())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached ||
                    entry.State == EntityState.Unchanged)
                    continue;



                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.GetTableName(),
                    ModifiedBy = CurrentUserId
                };

                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    var propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            auditEntry.AuditType = "Created";
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                auditEntry.AuditType = "Updated";

                            }
                            break;

                        case EntityState.Detached:
                        case EntityState.Unchanged:
                        case EntityState.Deleted:
                            auditEntry.AuditType = "Deleted";
                            break;
                        default:
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                Audits.Add(auditEntry.ToAudit());
            }

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries
                .Where(_ => _.HasTemporaryProperties)
                .ToList();
        }

        private Task OnAfterSaveChangesAsync(IReadOnlyCollection<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                // Save the Audit entry
                Audits.Add(auditEntry.ToAudit());
            }

            return base.SaveChangesAsync();
        }

        private void UpdateAuditEntities()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity<TKey> &&
                            (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                var entity = (IAuditableEntity<TKey>)entry.Entity;
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                    entity.CreatedBy = CurrentUserId;
                }
                else
                {
                    entity.UpdatedDate = now;
                    entity.UpdatedBy = CurrentUserId;
                    Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }
            }
        }

        private void SoftDelete()
        {
            if (!ModelBuilderExtensions.IsEnabledSoftDelete)
                return;

            ChangeTracker.DetectChanges();

            List<EntityEntry> markedAsDeleted = ChangeTracker
                .Entries()
                .Where(x => x.State == EntityState.Deleted)
                .ToList();

            foreach (EntityEntry entry in markedAsDeleted)
            {
                if (entry.State == EntityState.Deleted)
                {
                    Attach(entry.Entity);

                    foreach (var property in entry.Properties)
                    {
                        property.IsModified = false;
                    }

                    entry.CurrentValues["is_deleted"] = true;
                }
            }
        }
    }
}