namespace Carbon.Audit;

/// <summary>
/// Configuration settings for Carbon.Audit.
/// Bind this from the "CarbonAudit" section of your appsettings.json.
/// <code>
/// "CarbonAudit": {
///   "Enabled": true
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
}

