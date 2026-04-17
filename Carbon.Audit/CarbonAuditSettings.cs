namespace Carbon.Audit;

/// <summary>
/// Configuration settings for Carbon.Audit.
/// Bind this from the "CarbonAudit" section of your appsettings.json.
/// <code>
/// "CarbonAudit": {
///   "Enabled": true,
///   "ExcludedPaths": [ "/health", "/swagger" ],
///   "AllowedContentTypes": [ "application/json", "text/json" ],
///   "MaxRequestBodyBytes": 4096
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
    public string[] ExcludedPaths { get; set; }

    /// <summary>
    /// Gets or sets the list of allowed request content types for auditing.
    /// If empty or null, all content types are allowed.
    /// </summary>
    public string[] AllowedContentTypes { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed request body size (in bytes) for auditing.
    /// If null, size is unlimited.
    /// </summary>
    public long? MaxRequestBodyBytes { get; set; }
}