using MassTransit;

namespace Carbon.Audit.Bus;

/// <summary>
/// Dedicated MassTransit bus for publishing audit events.
/// Registered automatically by <see cref="Carbon.Audit.ServiceCollectionExtensions.AddCarbonAudit"/>
/// when no other <see cref="IBus"/> is found in the DI container.
/// </summary>
public interface IAuditBus : IBus { }
