using Carbon.Audit.Contracts;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Carbon.Audit.Producer;

internal sealed class AuditEntryPending
{
    public EntityEntry Entry { get; set; } = null!;
    public AuditEvent Event { get; set; } = null!;
}