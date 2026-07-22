using System.Reflection;
using Carbon.Audit.Contracts;
using Carbon.Audit.Producer.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Carbon.Audit.Producer;

public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly RequestContext _requestContext;
    private readonly ILogger<AuditInterceptor> _logger;

    /// <summary>
    /// Mirrors <c>CarbonAudit:Enabled</c>. The interceptor is registered in DI unconditionally
    /// so that DbContexts taking it as a constructor dependency stay resolvable on environments
    /// where auditing is turned off; this flag is what actually switches the behaviour off.
    /// When <c>false</c> every interception point returns immediately, so no change tracker
    /// traversal, reflection or allocation happens.
    /// </summary>
    private readonly bool _enabled;

    private readonly IHttpContextAccessor? _httpContextAccessor;

    private List<AuditEntryPending>? _pendingAudits;

    public AuditInterceptor(
        RequestContext ctx,
        ILogger<AuditInterceptor> logger,
        IOptions<CarbonAuditSettings> settings,
        IHttpContextAccessor httpContextAccessor)
    {
        _requestContext = ctx;
        _logger = logger;
        _enabled = settings?.Value?.Enabled ?? false;
        _httpContextAccessor = httpContextAccessor;
    }

    // -------------------------------------------------------------------------
    // Interception points
    //
    // EF Core never bridges the synchronous and asynchronous interception paths:
    // DbContext.SaveChanges() only triggers the sync overloads and SaveChangesAsync()
    // only the async ones. Both pairs are therefore implemented, delegating to the
    // shared handlers below - otherwise every synchronous save would silently produce
    // no audit event at all.
    // -------------------------------------------------------------------------

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        PrepareAudits(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken)
    {
        PrepareAudits(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        QueuePendingAudits();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken)
    {
        QueuePendingAudits();
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        QueuePendingAuditsAfterFailure();
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken)
    {
        QueuePendingAuditsAfterFailure();
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Shared handlers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Snapshots the auditable entries of the change tracker before they are persisted.
    /// </summary>
    private void PrepareAudits(DbContextEventData eventData)
    {
        if (!_enabled) return;

        // Audit events are published from a single place: RequestContextMiddleware, which only
        // runs inside an HTTP request scope. Saves that happen outside a request - database
        // seeding, Quartz jobs, MassTransit consumers - write into a RequestContext that is
        // never drained, so their events are discarded when the scope ends. Skipping them here
        // therefore changes nothing about what is actually published; it avoids the wasted
        // change tracker traversal and keeps PendingAuditEvents from growing without bound in
        // long-lived non-HTTP scopes.
        // The accessor itself is only null when the interceptor was constructed outside DI; in
        // that case collecting is the safer default.
        if (_httpContextAccessor is not null && _httpContextAccessor.HttpContext is null) return;

        if (_requestContext.IsHmiRequest) return;

        try
        {
            if (eventData.Context is not { } db) return;

            _pendingAudits = BuildPendingAudits(db.ChangeTracker.Entries());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AUDIT] Failed to build pending audit events");
        }
    }

    /// <summary>
    /// Hands the pending audit events over to the request context after a successful save,
    /// resolving the values that are only known once the database has assigned them.
    /// </summary>
    private void QueuePendingAudits()
    {
        if (!_enabled) return;
        if (_pendingAudits == null || _pendingAudits.Count == 0) return;

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

    /// <summary>
    /// Hands the pending audit events over to the request context after a failed save,
    /// so that rejected attempts are audited as well.
    /// </summary>
    private void QueuePendingAuditsAfterFailure()
    {
        if (!_enabled) return;
        if (_pendingAudits == null || _pendingAudits.Count == 0) return;

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
            Payload = _requestContext.HttpRequestAuditEnabled ? null : _requestContext.Payload,
            RequestAuditId = _requestContext.HttpRequestAuditEnabled ? _requestContext.RequestAuditId : null,
            
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