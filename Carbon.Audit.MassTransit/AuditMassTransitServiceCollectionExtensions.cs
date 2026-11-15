using Carbon.Audit.Bus;
using Carbon.Audit.Producer;
using Carbon.MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Carbon.Audit;

/// <summary>
/// Registers the MassTransit/RabbitMQ transport that publishes Carbon.Audit events.
/// </summary>
public static class AuditMassTransitServiceCollectionExtensions
{
    /// <summary>
    /// Registers a dedicated <see cref="IAuditBus"/> and the <see cref="IAuditEventPublisher"/>
    /// that publishes audit events through it.
    /// <para>
    /// No-op when <c>CarbonAudit:Enabled</c> is <c>false</c>: nothing connects to RabbitMQ when
    /// auditing is disabled. Safe to call more than once; only the first call registers.
    /// Also a no-op when an <see cref="IAuditEventPublisher"/> is already registered, so a
    /// service-provided publisher is kept and no unused audit bus is started.
    /// </para>
    /// <para>
    /// The bus is configured from the <c>MassTransit:RabbitMq</c> section of
    /// <paramref name="configuration"/>. <c>AddCarbonAudit</c> (called by Carbon.WebApplication)
    /// registers the transport-independent services; this method only adds the transport.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">
    /// Required. Must contain a valid <c>MassTransit:RabbitMq</c> section when
    /// <c>CarbonAudit:Enabled</c> is <c>true</c>.
    /// </param>
    public static IServiceCollection AddCarbonAuditMassTransit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        if (!configuration.IsCarbonAuditEnabled()) return services;
        if (services.Any(d => d.ServiceType == typeof(IAuditBus))) return services;
        // A service that brings its own publisher keeps it, and no unused bus connects to RabbitMQ.
        if (services.Any(d => d.ServiceType == typeof(IAuditEventPublisher))) return services;

        services.AddMassTransitBus<IAuditBus>(cfg =>
        {
            cfg.AddRabbitMqBus(configuration, (_, busFactoryConfig) =>
            {
                busFactoryConfig.PurgeOnStartup = false;
                busFactoryConfig.ConfigureAuditPublisher();
            });
        });

        services.TryAddScoped<IAuditEventPublisher>(sp =>
            new RabbitMqAuditEventPublisher(
                sp.GetRequiredService<IAuditBus>(),
                sp.GetRequiredService<ILogger<RabbitMqAuditEventPublisher>>()));

        return services;
    }
}
