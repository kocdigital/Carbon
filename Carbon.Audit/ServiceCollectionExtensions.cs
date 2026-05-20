using Carbon.Audit.Bus;
using Carbon.Audit.Producer;
using Carbon.Audit.Producer.Http;
using Carbon.MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Carbon.Audit;

/// <summary>
/// Extension methods for registering Carbon.Audit services into the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Carbon.Audit services.
    /// <para>
    /// Always registers a dedicated <see cref="IAuditBus"/> for audit event publishing.
    /// The bus is configured from the <c>MassTransit:RabbitMq</c> section of
    /// <paramref name="configuration"/>. <paramref name="configuration"/> is required.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">
    /// Required. Must contain a valid <c>MassTransit:RabbitMq</c> section.
    /// </param>
    public static IServiceCollection AddCarbonAudit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.AddHttpContextAccessor();
        services.AddScoped<RequestContext>();
        services.AddScoped<AuditInterceptor>();

        RegisterAuditBus(services, configuration);

        services.AddScoped<IAuditEventPublisher>(sp =>
            new RabbitMqAuditEventPublisher(
                sp.GetRequiredService<IAuditBus>(),
                sp.GetRequiredService<ILogger<RabbitMqAuditEventPublisher>>()));

        return services;
    }

    /// <summary>
    /// Adds the <see cref="RequestContextMiddleware"/> to the application pipeline.
    /// </summary>
    public static IApplicationBuilder UseCarbonAudit(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestContextMiddleware>();
        return app;
    }

    // -------------------------------------------------------------------------
    // Internals
    // -------------------------------------------------------------------------

    private static void RegisterAuditBus(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransitBus<IAuditBus>(cfg =>
        {
            cfg.AddRabbitMqBus(configuration, (_, busFactoryConfig) =>
            {
                busFactoryConfig.PurgeOnStartup = false;
                busFactoryConfig.ConfigureAuditPublisher();
            });
        });
    }
}

