using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text.Json;
using Carbon.Audit.Contracts;
using Carbon.Audit.Producer.Http;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Carbon.Audit.Producer;

public sealed class RequestContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RequestContextMiddleware> _logger;
    private static readonly string? ApiName = Assembly.GetEntryAssembly()?.GetName().Name;
    private static readonly HashSet<string> SensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Cookie",
        "Set-Cookie",
        "X-Api-Key",
        "Token"
    };

    public RequestContextMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<RequestContextMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
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

        var maxResponseBytes = _configuration.GetSection("CarbonAudit").GetValue<int?>("MaxResponseBodyBytes") ?? 4096;
        var originalBody = http.Response.Body;
        using var captureBuffer = new MemoryStream(maxResponseBytes);
        http.Response.Body = new TeeStream(originalBody, captureBuffer, maxResponseBytes);

        var stopwatch = Stopwatch.StartNew();
        Exception? pipelineException = null;

        try
        {
            await _next(http);
        }
        catch (Exception ex)
        {
            pipelineException = ex;
        }
        finally
        {
            stopwatch.Stop();
            http.Response.Body = originalBody;
        }

        ctx.HttpStatusCode = http.Response.StatusCode;

        if (captureBuffer.Length > 0)
        {
            var contentType = http.Response.ContentType;
            if (contentType != null && contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                captureBuffer.Position = 0;
                var responseBodyText = Encoding.UTF8.GetString(captureBuffer.GetBuffer(), 0, (int)captureBuffer.Length);
                var (apiStatusCode, errorCode, messages) = ExtractResponseErrorInfo(responseBodyText);
                ctx.ResponseApiStatusCode = apiStatusCode;
                ctx.ResponseErrorCode = errorCode;
                ctx.ResponseMessages = messages;
            }
        }

        if (ctx.PendingAuditEvents.Count > 0)
        {
            if (!ctx.HttpRequestAuditEnabled)
            {
                foreach (var evt in ctx.PendingAuditEvents)
                {
                    if (evt is null) continue;
                    evt.Payload = ctx.Payload;
                    evt.HttpStatusCode = ctx.HttpStatusCode;
                }
            }

            foreach (var evt in ctx.PendingAuditEvents)
            {
                if (evt is null) continue;
                evt.ApiStatusCode = ctx.ResponseApiStatusCode;
                evt.ErrorCode = ctx.ResponseErrorCode;
                evt.Messages = ctx.ResponseMessages;
            }

            await publisher.PublishBatchAsync(ctx.PendingAuditEvents);
            ctx.PendingAuditEvents.Clear();
        }

        Exception? publishException = null;
        var statusCodeFilter = _configuration.GetSection("CarbonAudit:HttpStatusCodeFilter").Get<HttpStatusCodeFilter>();
        var shouldPublish = statusCodeFilter is null || statusCodeFilter.IsEmpty ||
                            statusCodeFilter.Matches(http.Response.StatusCode);
        if (ctx.HttpRequestAuditEnabled && shouldPublish)
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
                    HttpStatusCode = http.Response.StatusCode,
                    CorrelationId = ctx.CorrelationId,
                    Payload = ctx.Payload,
                    Headers = BuildHeaders(http.Request.Headers),
                    UserId = ctx.UserId,
                    UserName = ctx.UserName,
                    UserEmail = ctx.UserEmail,
                    IpAddress = ctx.IpAddress,
                    SessionId = ctx.SessionId,
                    ClientSource = ctx.Source,
                    ApiStatusCode = ctx.ResponseApiStatusCode,
                    ErrorCode = ctx.ResponseErrorCode,
                    Messages = ctx.ResponseMessages,
                    DurationMs = stopwatch.ElapsedMilliseconds
                });
            }
            catch (Exception ex)
            {
                publishException = ex;
            }

        }

        if (publishException is not null)
            _logger.LogError(publishException, "[AUDIT] Failed to publish request audit event");

        if (pipelineException is not null)
            ExceptionDispatchInfo.Capture(pipelineException).Throw();
    }

    private static Dictionary<string, string> BuildHeaders(IHeaderDictionary headers)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in headers)
        {
            result[header.Key] = SensitiveHeaders.Contains(header.Key)
                ? "[REDACTED]"
                : string.Join(",", header.Value.Where(v => v is not null).ToArray());
        }

        return result;
    }

    private static (int? apiStatusCode, int? errorCode, List<string>? messages) ExtractResponseErrorInfo(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody)) return (null, null, null);

        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object) return (null, null, null);

            int? apiStatusCode = null;
            int? errorCode = null;
            List<string>? messages = null;

            if (TryGetPropertyIgnoreCase(root, "statusCode", out var scElem) &&
                scElem.ValueKind == JsonValueKind.Number &&
                scElem.TryGetInt32(out var sc))
                apiStatusCode = sc;

            if (TryGetPropertyIgnoreCase(root, "errorCode", out var ecElem) &&
                ecElem.ValueKind == JsonValueKind.Number &&
                ecElem.TryGetInt32(out var ec))
                errorCode = ec;

            if (TryGetPropertyIgnoreCase(root, "messages", out var msgsElem))
                messages = ParseStringArray(msgsElem);

            if (messages == null &&
                TryGetPropertyIgnoreCase(root, "data", out var dataElem) &&
                dataElem.ValueKind == JsonValueKind.Object &&
                TryGetPropertyIgnoreCase(dataElem, "messages", out var dataMsgsElem))
            {
                messages = ParseStringArray(dataMsgsElem);
            }

            return (apiStatusCode, errorCode, messages);
        }
        catch
        {
            return (null, null, null);
        }
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string name, out JsonElement value)
    {
        foreach (var prop in element.EnumerateObject())
        {
            if (prop.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                value = prop.Value;
                return true;
            }
        }
        value = default;
        return false;
    }

    private static List<string>? ParseStringArray(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Array) return null;
        var list = new List<string>();
        foreach (var item in element.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.String)
            {
                var s = item.GetString();
                if (s is not null)
                    list.Add(s);
            }
        }
        return list.Count > 0 ? list : null;
    }

    /// <summary>
    /// A write-through stream that forwards all writes to the original response stream
    /// while capturing up to <c>maxCaptureBytes</c> into a bounded in-memory buffer.
    /// This avoids buffering the full response in memory and lets the client receive
    /// data immediately, while still allowing a bounded prefix to be read for JSON parsing.
    /// </summary>
    private sealed class TeeStream : Stream
    {
        private readonly Stream _inner;
        private readonly MemoryStream _capture;
        private readonly int _maxCaptureBytes;
        private readonly object _captureLock = new();
        private int _capturedBytes;

        internal TeeStream(Stream inner, MemoryStream capture, int maxCaptureBytes)
        {
            _inner = inner;
            _capture = capture;
            _maxCaptureBytes = maxCaptureBytes;
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() => _inner.Flush();
        public override Task FlushAsync(CancellationToken cancellationToken) => _inner.FlushAsync(cancellationToken);

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
            CaptureBytes(new ReadOnlySpan<byte>(buffer, offset, count));
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _inner.WriteAsync(buffer, offset, count, cancellationToken);
            CaptureBytes(new ReadOnlySpan<byte>(buffer, offset, count));
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await _inner.WriteAsync(buffer, cancellationToken);
            CaptureBytes(buffer.Span);
        }

        private void CaptureBytes(ReadOnlySpan<byte> data)
        {
            lock (_captureLock)
            {
                if (_capturedBytes >= _maxCaptureBytes) return;
                var toCapture = Math.Min(data.Length, _maxCaptureBytes - _capturedBytes);
                _capture.Write(data.Slice(0, toCapture));
                _capturedBytes += toCapture;
            }
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            // Do not dispose _inner — we do not own the original response stream.
            base.Dispose(disposing);
        }
    }

}
