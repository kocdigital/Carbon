using System.Reflection;
using Carbon.Audit.Contracts;
using Carbon.Audit.Producer.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Carbon.Audit.Producer;

public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly RequestContext _requestContext;
    private readonly ILogger<AuditInterceptor> _logger;

    private List<AuditEntryPending>? _pendingAudits;

    public AuditInterceptor(
        RequestContext ctx,
        ILogger<AuditInterceptor> logger)
    {
        _requestContext = ctx;
        _logger = logger;
    }
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken)
    {
        if (_requestContext.IsHmiRequest)
            return base.SavingChangesAsync(eventData, result, cancellationToken); 

        try
        {
            if (eventData.Context is not { } db)
                return base.SavingChangesAsync(eventData, result, cancellationToken); 
            
            _pendingAudits = BuildPendingAudits(db.ChangeTracker.Entries());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AUDIT] Failed to build pending audit events");
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken); 
    }
    
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken)
    {
        if (_pendingAudits != null && _pendingAudits.Count > 0)
        {
            try
            {
                foreach (var pending in _pendingAudits)
                {
                    var evt = pending.Event;

                    if (evt.Action == AuditAction.Created)
                    {
                        evt.EntityId = GetPrimaryKey(pending.Entry);
                        evt.After = GetSnapshot(pending.Entry, original: false);
                    }

                    _requestContext.PendingAuditEvents.Add(evt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AUDIT] Failed to queue audit events after save");
            }
            finally
            {
                _pendingAudits.Clear();
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
    
    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken)
    {
        if (_pendingAudits != null && _pendingAudits.Count > 0)
        {
            try
            {
                foreach (var pending in _pendingAudits)
                    _requestContext.PendingAuditEvents.Add(pending.Event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AUDIT] Failed to queue audit events after save failure");
            }
            finally
            {
                _pendingAudits.Clear();
            }
        }

        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }
    
    private List<AuditEntryPending> BuildPendingAudits(IEnumerable<EntityEntry> entries)
    {
        return entries
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => e.Entity.GetType().IsDefined(typeof(AuditableAttribute), inherit: true))
            .Select(e => new AuditEntryPending { Entry = e, Event = BuildEvent(e) }) 
            .ToList();
    }

    private AuditEvent BuildEvent(EntityEntry entry)
    {
        var entityType = entry.Entity.GetType();
        var action = MapAction(entry.State);

        return new AuditEvent
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            UserId = _requestContext.UserId,
            UserName = _requestContext.UserName,
            UserEmail = _requestContext.UserEmail,
            IpAddress = _requestContext.IpAddress,
            SessionId = _requestContext.SessionId,
            ClientSource = _requestContext.Source,
            CorrelationId = _requestContext.CorrelationId,
            Endpoint = _requestContext.Endpoint,
            Payload = _requestContext.Payload,
            
            Service = Assembly.GetEntryAssembly()?.GetName().Name,
            EntityType = entityType.Name,
            EntityId = GetPrimaryKey(entry),
            EntityName = GetEntityName(entry),
            
            Action = action,
            Before = action != AuditAction.Created ? GetSnapshot(entry, original: true) : null,
            
            After = action == AuditAction.Updated ? GetSnapshot(entry, original: false) : null,
            
            Changes = action == AuditAction.Updated ? GetFieldDiffs(entry) : new(),
        };
    }
    
    private static HashSet<string> GetIgnoredProperties(EntityEntry entry)
    {
        return entry.Entity.GetType().GetProperties()
            .Where(p => p.IsDefined(typeof(AuditIgnoreAttribute), true))
            .Select(p => p.Name)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static Dictionary<string, object?> GetSnapshot(EntityEntry entry, bool original)
    {
        var ignoredProps = GetIgnoredProperties(entry);

        return entry.Properties
            .Where(p => !ignoredProps.Contains(p.Metadata.Name))
            .ToDictionary(
                p => p.Metadata.Name,
                p => original ? p.OriginalValue : p.CurrentValue);
    }

    private static List<FieldChange> GetFieldDiffs(EntityEntry entry)
    {
        var ignoredProps = GetIgnoredProperties(entry);

        return entry.Properties
            .Where(p => p.IsModified)
            .Where(p => !ignoredProps.Contains(p.Metadata.Name))
            .Select(p => new FieldChange
            {
                Field = p.Metadata.Name,
                Before = p.OriginalValue, 
                After = p.CurrentValue
            })
            .ToList();
    }

    private static string GetPrimaryKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key == null) return string.Empty;
        
        var values = key.Properties.Select(p => entry.Property(p.Name).CurrentValue ?? entry.Property(p.Name).OriginalValue);
        return string.Join("_", values);
    }

    private static string GetEntityName(EntityEntry entry)
    {
        foreach (var prop in new[] { "Name", "Title", "Code", "Number" })
        {
            var p = entry.Properties.FirstOrDefault(x => x.Metadata.Name == prop);
            if (p?.CurrentValue is string s && !string.IsNullOrEmpty(s)) return s;
        }

        return GetPrimaryKey(entry);
    }

    private static AuditAction MapAction(EntityState state) => state switch
    {
        EntityState.Added => AuditAction.Created,
        EntityState.Modified => AuditAction.Updated,
        EntityState.Deleted => AuditAction.Deleted,
        _ => throw new ArgumentOutOfRangeException(nameof(state))
    };
}