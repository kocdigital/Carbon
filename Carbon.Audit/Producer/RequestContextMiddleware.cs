using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
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
    private static readonly string? ApiName = Assembly.GetEntryAssembly()?.GetName().Name;
    private static readonly HashSet<string> SensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Cookie",
        "Set-Cookie",
        "X-Api-Key"
    };

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
        
        if (method is "POST" or "PUT" or "PATCH" or "DELETE")
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
            ?? http.Request.Headers["CorrelationId"].FirstOrDefault()
            ?? http.Request.Headers["TransactionId"].FirstOrDefault();

        ctx.CorrelationId = string.IsNullOrWhiteSpace(corr) ? null : corr.Trim();

        ctx.HttpRequestAuditEnabled = _configuration.GetSection("CarbonAudit").GetValue<bool>("HttpRequestAuditEnabled");

        Exception? pipelineException = null;
        
        try
        {
            await _next(http);
        }
        catch (Exception ex)
        {
            pipelineException = ex;
        }

        ctx.HttpStatusCode = DetermineHttpStatusCode(http.Response.StatusCode);

        if (ctx.PendingAuditEvents.Count > 0)
        {
            if (!ctx.HttpRequestAuditEnabled)
            {
                foreach (var evt in ctx.PendingAuditEvents)
                {
                    evt.Payload = ctx.Payload;
                    evt.HttpStatusCode = ctx.HttpStatusCode;
                }
            }

            await publisher.PublishBatchAsync(ctx.PendingAuditEvents);
            ctx.PendingAuditEvents.Clear();
        }

        Exception? publishException = null;
        if (ctx.HttpRequestAuditEnabled)
        {
            try
            {
                await publisher.PublishRequestAsync(new HttpRequestAuditEvent
                {
                    Id = Guid.NewGuid(),
                    RequestAuditId = ctx.RequestAuditId,
                    Timestamp = DateTime.UtcNow,
                    ApiName = ApiName,
                    Endpoint = ctx.Endpoint,
                    HttpMethod = method,
                    HttpStatusCode = ctx.HttpStatusCode ?? StatusCodes.Status500InternalServerError,
                    CorrelationId = ctx.CorrelationId,
                    Payload = ctx.Payload,
                    Headers = BuildHeaders(http.Request.Headers),
                    UserId = ctx.UserId,
                    UserName = ctx.UserName,
                    UserEmail = ctx.UserEmail,
                    IpAddress = ctx.IpAddress,
                    SessionId = ctx.SessionId,
                    ClientSource = ctx.Source
                });
            }
            catch (Exception ex)
            {
                publishException = ex;
            }
        }

        if (pipelineException is not null && publishException is not null)
            throw new AggregateException(pipelineException, publishException);

        if (pipelineException is not null)
            ExceptionDispatchInfo.Capture(pipelineException).Throw();

        if (publishException is not null)
            ExceptionDispatchInfo.Capture(publishException).Throw();
    }

    private static Dictionary<string, string> BuildHeaders(IHeaderDictionary headers)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in headers)
        {
            result[header.Key] = SensitiveHeaders.Contains(header.Key)
                ? "[REDACTED]"
                : string.Join(",", header.Value.ToArray());
        }

        return result;
    }

    private static int DetermineHttpStatusCode(int responseStatusCode)
    {
        return responseStatusCode;
    }
}
