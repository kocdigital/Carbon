namespace Carbon.Audit.Contracts;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class AuditIgnoreAttribute : Attribute
{
    
}