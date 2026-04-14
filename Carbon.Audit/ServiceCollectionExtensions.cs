using Carbon.Audit.Producer;
using Carbon.Audit.Producer.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.Audit;

/// <summary>
/// Extension methods for registering Carbon.Audit services into the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="RequestContext"/> as scoped and <see cref="AuditInterceptor"/> as scoped.
    /// </summary>
    public static IServiceCollection AddCarbonAudit(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<RequestContext>();
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<IAuditEventPublisher, RabbitMqAuditEventPublisher>();
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
}

