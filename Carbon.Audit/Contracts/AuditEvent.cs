using System.Text.Json.Serialization;

namespace Carbon.Audit.Contracts;

public sealed class AuditEvent
{
    public Guid Id { get; set; }
    
    public DateTime Timestamp { get; set; }

    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? IpAddress { get; set; }
    public string? SessionId { get; set; }

    [JsonPropertyName("clientSource")]
    public ClientSource? ClientSource { get; set; }
    
    public string? CorrelationId { get; set; }

    public string? Service { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? EntityName { get; set; }

    public AuditAction Action { get; set; }
    
    public object? Before { get; set; }
    public object? After { get; set; }

    public List<FieldChange> Changes { get; set; } = new();
}
