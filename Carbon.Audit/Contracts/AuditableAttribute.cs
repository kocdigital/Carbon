namespace Carbon.Audit.Contracts;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class AuditableAttribute : Attribute
{
    
}