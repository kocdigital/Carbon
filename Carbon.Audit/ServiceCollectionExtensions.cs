using Carbon.Audit.Producer;
using Carbon.Audit.Producer.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
    /// Registers the transport-independent Carbon.Audit services.
    /// <para>
    /// <see cref="RequestContext"/> and <see cref="AuditInterceptor"/> are always registered,
    /// regardless of <c>CarbonAudit:Enabled</c>. Consuming services may take
    /// <see cref="AuditInterceptor"/> as a <see cref="Microsoft.EntityFrameworkCore.DbContext"/>
    /// constructor dependency, and DI resolution must never depend on configuration - otherwise
    /// the whole application fails to start on environments where auditing is turned off.
    /// When disabled the interceptor is inert (see <see cref="AuditInterceptor"/>).
    /// </para>
    /// <para>
    /// This method registers no message transport. When <c>CarbonAudit:Enabled</c> is
    /// <c>true</c>, an <see cref="IAuditEventPublisher"/> must also be registered, normally by
    /// calling <c>services.AddCarbonAuditMassTransit(configuration)</c> from the transport package
    /// that matches the service's MassTransit version: <c>Carbon.Audit.MassTransit</c> for
    /// MassTransit 8, <c>Carbon.Audit.MassTransit7</c> for MassTransit 7.
    /// <see cref="UseCarbonAudit"/> fails at startup when it is missing.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Required. The configuration holding the <c>CarbonAudit</c> section.</param>
    public static IServiceCollection AddCarbonAudit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.Configure<CarbonAuditSettings>(configuration.GetSection(SectionName));

        services.AddHttpContextAccessor();
        services.TryAddScoped<RequestContext>();
        services.TryAddScoped<AuditInterceptor>();

        return services;
    }

    /// <summary>
    /// Returns whether <c>CarbonAudit:Enabled</c> is <c>true</c> in <paramref name="configuration"/>.
    /// </summary>
    public static bool IsCarbonAuditEnabled(this IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        return configuration.GetValue<bool>($"{SectionName}:Enabled");
    }

    /// <summary>
    /// Adds the <see cref="RequestContextMiddleware"/> to the application pipeline.
    /// <para>
    /// No-op when <c>CarbonAudit:Enabled</c> is <c>false</c>.
    /// </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Auditing is enabled but no <see cref="IAuditEventPublisher"/> is registered. Failing here,
    /// while the pipeline is built, stops a misconfigured service at startup instead of letting it
    /// run and lose every audit event.
    /// </exception>
    public static IApplicationBuilder UseCarbonAudit(this IApplicationBuilder app)
    {
        var configuration = app.ApplicationServices.GetService<IConfiguration>();
        var enabled = configuration?.IsCarbonAuditEnabled() ?? false;
        if (!enabled) return app;

        EnsurePublisherRegistered(app.ApplicationServices);

        app.UseMiddleware<RequestContextMiddleware>();
        return app;
    }

    private static void EnsurePublisherRegistered(IServiceProvider services)
    {
        var isService = services.GetService<IServiceProviderIsService>();
        var registered = isService != null
            ? isService.IsService(typeof(IAuditEventPublisher))
            : HasPublisherInScope(services);

        if (registered) return;

        throw new InvalidOperationException(
            $"\"{SectionName}:Enabled\" is true but no {nameof(IAuditEventPublisher)} is registered, " +
            "so audit events would be lost. Reference the Carbon.Audit transport package that matches " +
            "this service's MassTransit version - Carbon.Audit.MassTransit for MassTransit 8, " +
            "Carbon.Audit.MassTransit7 for MassTransit 7 - and call " +
            "services.AddCarbonAuditMassTransit(Configuration) in ConfigureServices, " +
            $"or set \"{SectionName}:Enabled\" to false.");
    }

    // Fallback for containers that do not implement IServiceProviderIsService.
    private static bool HasPublisherInScope(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        return scope.ServiceProvider.GetService<IAuditEventPublisher>() != null;
    }
}
