using Carbon.Audit.Bus;
using Carbon.Audit.Producer;
using Carbon.Audit.Producer.Http;
using Carbon.MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Carbon.Audit;

/// <summary>
/// Extension methods for registering Carbon.Audit services into the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The configuration section Carbon.Audit is bound from.
    /// </summary>
    public const string SectionName = "CarbonAudit";

    /// <summary>
    /// Registers Carbon.Audit services.
    /// <para>
    /// <see cref="RequestContext"/> and <see cref="AuditInterceptor"/> are always registered,
    /// regardless of <c>CarbonAudit:Enabled</c>. Consuming services may take
    /// <see cref="AuditInterceptor"/> as a <see cref="Microsoft.EntityFrameworkCore.DbContext"/>
    /// constructor dependency, and DI resolution must never depend on configuration - otherwise
    /// the whole application fails to start on environments where auditing is turned off.
    /// When disabled the interceptor is inert (see <see cref="AuditInterceptor"/>) and these
    /// registrations pull in no infrastructure: no message bus connection is established.
    /// </para>
    /// <para>
    /// The audit bus and the event publisher are registered only when the feature is enabled.
    /// The bus is configured from the <c>MassTransit:RabbitMq</c> section of
    /// <paramref name="configuration"/>. <paramref name="configuration"/> is required.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">
    /// Required. Must contain a valid <c>MassTransit:RabbitMq</c> section when
    /// <c>CarbonAudit:Enabled</c> is <c>true</c>.
    /// </param>
    public static IServiceCollection AddCarbonAudit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.Configure<CarbonAuditSettings>(configuration.GetSection(SectionName));

        services.AddHttpContextAccessor();
        services.TryAddScoped<RequestContext>();
        services.TryAddScoped<AuditInterceptor>();

        var _useCarbonAudit = configuration.GetValue<bool>($"{SectionName}:Enabled");
        if (!_useCarbonAudit) return services;

        RegisterAuditBus(services, configuration);

        services.AddScoped<IAuditEventPublisher>(sp =>
            new RabbitMqAuditEventPublisher(
                sp.GetRequiredService<IAuditBus>(),
                sp.GetRequiredService<ILogger<RabbitMqAuditEventPublisher>>()));

        return services;
    }

    /// <summary>
    /// Adds the <see cref="RequestContextMiddleware"/> to the application pipeline.
    /// <para>
    /// No-op when <c>CarbonAudit:Enabled</c> is <c>false</c>: the middleware depends on
    /// <see cref="IAuditEventPublisher"/>, which is only registered when the feature is enabled.
    /// </para>
    /// </summary>
    public static IApplicationBuilder UseCarbonAudit(this IApplicationBuilder app)
    {
        var configuration = app.ApplicationServices.GetService<IConfiguration>();
        var enabled = configuration?.GetValue<bool>($"{SectionName}:Enabled") ?? false;
        if (!enabled) return app;

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

