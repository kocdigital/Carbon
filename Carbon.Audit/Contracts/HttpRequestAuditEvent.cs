using System.Text.Json.Serialization;

namespace Carbon.Audit.Contracts;

public sealed class HttpRequestAuditEvent
{
    public Guid Id { get; set; }
    public Guid RequestAuditId { get; set; }
    public DateTime Timestamp { get; set; }

    public string? ApiName { get; set; }
    public string? Endpoint { get; set; }
    public string? HttpMethod { get; set; }
    public int HttpStatusCode { get; set; }

    public string? CorrelationId { get; set; }
    public string? Payload { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();

    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? IpAddress { get; set; }
    public string? SessionId { get; set; }

    [JsonPropertyName("clientSource")]
    public ClientSource? ClientSource { get; set; }
}
