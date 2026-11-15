using MassTransit;

namespace Carbon.Audit.Bus;

/// <summary>
/// Dedicated MassTransit bus for publishing audit events.
/// Registered by <see cref="Carbon.Audit.AuditMassTransitServiceCollectionExtensions.AddCarbonAuditMassTransit"/>
/// when <c>CarbonAudit:Enabled</c> is <c>true</c>.
/// </summary>
public interface IAuditBus : IBus { }
