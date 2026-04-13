using System.Security.Claims;
using Carbon.Audit.Contracts;
using Carbon.Audit.Producer.Http;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Carbon.Audit.Producer;

public sealed class RequestContextMiddleware
{
    private readonly RequestDelegate _next;

    public RequestContextMiddleware(RequestDelegate next) => _next = next;

    private static string? ClaimValue(ClaimsPrincipal principal, string claimType)
        => principal.FindFirst(claimType)?.Value;

    public async Task InvokeAsync(HttpContext http, RequestContext ctx)
    {
        var endpoint = http.GetEndpoint();
        ctx.Endpoint = endpoint?.DisplayName ?? http.Request.Path;
        
        var method = http.Request.Method.ToUpperInvariant();
        
        if (method is "POST" or "PUT" or "DELETE")
        {
            http.Request.EnableBuffering(); 
            
            using (var reader = new StreamReader(http.Request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                ctx.Payload = body;
                
                http.Request.Body.Position = 0;
            }
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
    }
}