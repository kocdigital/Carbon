using Carbon.Audit.Contracts;

namespace Carbon.Audit.Producer.Http;

public sealed class RequestContext
{
    public string UserId { get; set; } = "system";
    public string UserName { get; set; } = "System";
    public string UserEmail { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    
    public string IpAddress { get; set; } = string.Empty;
    
    public ClientSource Source { get; set; } = ClientSource.Platform;
    public string? CorrelationId { get; set; }

    public bool IsHmiRequest => Source == ClientSource.HMI;
}