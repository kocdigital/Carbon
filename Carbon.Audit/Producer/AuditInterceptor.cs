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
    private readonly IAuditEventPublisher _publisher;
    private readonly ILogger<AuditInterceptor> _logger;

    public AuditInterceptor(
        RequestContext ctx,
        IAuditEventPublisher publisher,
        ILogger<AuditInterceptor> logger)
    {
        _requestContext = ctx;
        _publisher = publisher;
        _logger = logger;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        if (_requestContext.IsHmiRequest)
            return await base.SavingChangesAsync(eventData, result, ct);

        try
        {
            if (eventData.Context is not { } db)
                return await base.SavingChangesAsync(eventData, result, ct);

            var events = BuildAuditEvents(db.ChangeTracker.Entries());

            if (events.Count > 0)
                _ = _publisher.PublishBatchAsync(events); // fire-and-forget
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AUDIT] Failed to build audit events");
        }

        return await base.SavingChangesAsync(eventData, result, ct);
    }

    private List<AuditEvent> BuildAuditEvents(IEnumerable<EntityEntry> entries)
    {
        return entries
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => e.Entity.GetType().IsDefined(typeof(AuditableAttribute), inherit: true))
            .Select(BuildEvent)
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
            After = action != AuditAction.Deleted ? GetSnapshot(entry, original: false) : null,
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
                Before = p.OriginalValue?.ToString(),
                After = p.CurrentValue?.ToString()
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