namespace Carbon.Audit;

/// <summary>
/// Configuration settings for Carbon.Audit.
/// Bind this from the "CarbonAudit" section of your appsettings.json.
/// <code>
/// "CarbonAudit": {
///   "Enabled": true,
///   "ExcludedPaths": [ "/health", "/swagger" ],
///   "AllowedContentTypes": [ "application/json", "text/json" ],
///   "MaxRequestBodyBytes": 4096,
///   "HttpRequestAuditEnabled": true,
///   "HttpStatusCodeFilter": {
///     "ExactCodes": [ 401, 500 ],
///     "RangeStart": 400,
///     "RangeEnd": 499
///   }
/// }
/// </code>
/// </summary>
public class CarbonAuditSettings
{
    /// <summary>
    /// Gets or sets whether Carbon.Audit is enabled.
    /// When <c>true</c>, the audit interceptor and middleware are registered automatically.
    /// Default is <c>false</c>.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the list of request paths to exclude from auditing.
    /// </summary>
    public string[] ExcludedPaths { get; set; } = System.Array.Empty<string>();

    /// <summary>
    /// Gets or sets the list of allowed request content types for auditing.
    /// If empty or null, all content types are allowed.
    /// </summary>
    public string[] AllowedContentTypes { get; set; } = System.Array.Empty<string>();

    /// <summary>
    /// Gets or sets the maximum allowed request body size (in bytes) for auditing.
    /// If null, size is unlimited.
    /// </summary>
    public long? MaxRequestBodyBytes { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code filter for HTTP request audit events.
    /// When configured, only requests whose response status code matches are published.
    /// If null or empty, all status codes are published.
    /// <para>
    /// You can combine <see cref="HttpStatusCodeFilter.ExactCodes"/> (e.g. 401, 500)
    /// and/or a range via <see cref="HttpStatusCodeFilter.RangeStart"/> /
    /// <see cref="HttpStatusCodeFilter.RangeEnd"/> (e.g. 400–499 for all 4xx).
    /// </para>
    /// </summary>
    public HttpStatusCodeFilter? HttpStatusCodeFilter { get; set; }
}

/// <summary>
/// Defines which HTTP status codes should be included in HTTP request audit events.
/// </summary>
public class HttpStatusCodeFilter
{
    /// <summary>
    /// An explicit list of HTTP status codes to include (e.g. [401, 500]).
    /// </summary>
    public int[] ExactCodes { get; set; } = System.Array.Empty<int>();

    /// <summary>
    /// The inclusive lower bound of an HTTP status code range to include (e.g. 400).
    /// When set without <see cref="RangeEnd"/>, matches all codes &gt;= this value.
    /// </summary>
    public int? RangeStart { get; set; }

    /// <summary>
    /// The inclusive upper bound of an HTTP status code range to include (e.g. 499).
    /// When set without <see cref="RangeStart"/>, matches all codes &lt;= this value.
    /// </summary>
    public int? RangeEnd { get; set; }

    /// <summary>
    /// Returns <c>true</c> if the given status code satisfies this filter.
    /// </summary>
    public bool Matches(int statusCode)
    {
        if (ExactCodes.Length > 0 && System.Array.IndexOf(ExactCodes, statusCode) >= 0)
            return true;

        if (RangeStart.HasValue && RangeEnd.HasValue &&
            statusCode >= RangeStart.Value && statusCode <= RangeEnd.Value)
            return true;

        if (RangeStart.HasValue && !RangeEnd.HasValue && statusCode >= RangeStart.Value)
            return true;

        if (!RangeStart.HasValue && RangeEnd.HasValue && statusCode <= RangeEnd.Value)
            return true;

        return false;
    }

    /// <summary>
    /// Returns <c>true</c> when no filter criteria are defined (pass-through).
    /// </summary>
    public bool IsEmpty => ExactCodes.Length == 0 && !RangeStart.HasValue && !RangeEnd.HasValue;
}