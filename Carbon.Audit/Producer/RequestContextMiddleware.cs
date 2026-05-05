using System.Net;
using System.Security.Claims;
using Carbon.Audit.Contracts;
using Carbon.Audit.Producer.Http;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Carbon.Audit.Producer;

public sealed class RequestContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public RequestContextMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    private static string? ClaimValue(ClaimsPrincipal principal, string claimType)
        => principal.FindFirst(claimType)?.Value;

    public async Task InvokeAsync(HttpContext http, RequestContext ctx, IAuditEventPublisher publisher)
    {
        var endpoint = http.GetEndpoint();
        ctx.Endpoint = endpoint?.DisplayName ?? http.Request.Path;
        
        var method = http.Request.Method.ToUpperInvariant();
        
        if (method is "POST" or "PUT" or "DELETE")
        {
            http.Request.EnableBuffering(); 
            var maxBytes = _configuration.GetSection("CarbonAudit").GetValue<int?>("MaxRequestBodyBytes") ?? 1024;
            var buffer = new byte[maxBytes];
            int readBytes = await http.Request.Body.ReadAsync(buffer, 0, maxBytes);
            ctx.Payload = Encoding.UTF8.GetString(buffer, 0, readBytes);
            http.Request.Body.Position = 0;
        }
        
        var user = http.User;

        ctx.UserId =
            ClaimValue(user, ClaimTypes.NameIdentifier) ??
            ClaimValue(user, "sub") ??
            "anonymous";

        ctx.UserName =
            ClaimValue(user, ClaimTypes.Name) ??
            ClaimValue(user, "name") ??
            ClaimValue(user, "preferred_username") ??
            "Unknown";

        ctx.UserEmail =
            ClaimValue(user, ClaimTypes.Email) ??
            ClaimValue(user, "email") ??
            string.Empty;

        ctx.SessionId =
            ClaimValue(user, "sid") ??
            ClaimValue(user, "jti") ??  
            string.Empty;
        
        var remoteIp = http.Connection.RemoteIpAddress;
        string? ip = remoteIp?.ToString();
        
        if (remoteIp == null || IPAddress.IsLoopback(remoteIp))
        {
            var forwardedHeader = http.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedHeader))
            {
                ip = forwardedHeader.Split(',').FirstOrDefault()?.Trim();
            }
        }
        
        ctx.IpAddress = ip ?? string.Empty;

        var clientType = http.Request.Headers["X-Client-Type"].FirstOrDefault();
        ctx.Source = clientType?.Equals("HMI", StringComparison.OrdinalIgnoreCase) == true
            ? ClientSource.HMI
            : ClientSource.Platform;
        
        var corr =
            http.Request.Headers["X-CorrelationId"].FirstOrDefault()
            ?? http.Request.Headers["X-Correlation-Id"].FirstOrDefault()
            ?? http.Request.Headers["correlationId"].FirstOrDefault()
            ?? http.Request.Headers["CorrelationId"].FirstOrDefault();

        ctx.CorrelationId = string.IsNullOrWhiteSpace(corr) ? null : corr.Trim();

        await _next(http);

        ctx.HttpStatusCode = http.Response.StatusCode;

        if (ctx.PendingAuditEvents.Count == 0 && ctx.HttpStatusCode >= 400)
        {
            ctx.PendingAuditEvents.Add(new AuditEvent
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                UserId = ctx.UserId,
                UserName = ctx.UserName,
                UserEmail = ctx.UserEmail,
                IpAddress = ctx.IpAddress,
                SessionId = ctx.SessionId,
                ClientSource = ctx.Source,
                CorrelationId = ctx.CorrelationId,
                Endpoint = ctx.Endpoint,
                Payload = ctx.Payload,
                Action = AuditAction.FailedRequest,
                HttpStatusCode = ctx.HttpStatusCode
            });
        }

        if (ctx.PendingAuditEvents.Count > 0)
        {
            foreach (var evt in ctx.PendingAuditEvents)
                evt.HttpStatusCode = ctx.HttpStatusCode;

            await publisher.PublishBatchAsync(ctx.PendingAuditEvents);
            ctx.PendingAuditEvents.Clear();
        }
    }
}