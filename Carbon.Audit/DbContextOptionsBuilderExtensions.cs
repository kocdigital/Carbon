using Carbon.Audit.Producer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.Audit;

/// <summary>
/// Extension methods for wiring Carbon.Audit into a <see cref="DbContext"/>.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="AuditInterceptor"/> to the <see cref="DbContext"/> being configured.
    /// <para>
    /// This is the supported way to wire auditing into a service that registers its
    /// <see cref="DbContext"/> itself. Do <b>not</b> take <see cref="AuditInterceptor"/> as a
    /// <see cref="DbContext"/> constructor dependency: that ties context resolution to the audit
    /// configuration and makes the context harder to construct from design-time tooling,
    /// context pooling and tests.
    /// </para>
    /// <para>
    /// Safe to call unconditionally. No <c>CarbonAudit:Enabled</c> check is needed: when auditing
    /// is disabled the interceptor is inert, and when Carbon.Audit was never registered at all the
    /// interceptor simply resolves to <c>null</c> and nothing is added.
    /// </para>
    /// <example>
    /// <code>
    /// services.AddDbContext&lt;MyContext&gt;((sp, options) =>
    /// {
    ///     options.UseNpgsql(connectionString);
    ///     options.AddCarbonAuditInterceptor(sp);
    /// });
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="options">The options builder of the context being configured.</param>
    /// <param name="serviceProvider">
    /// The provider handed to the <c>AddDbContext</c> callback. The interceptor is scoped, so it
    /// must be resolved from this provider rather than from the root one.
    /// </param>
    public static DbContextOptionsBuilder AddCarbonAuditInterceptor(
        this DbContextOptionsBuilder options,
        IServiceProvider serviceProvider)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

        var interceptor = serviceProvider.GetService<AuditInterceptor>();
        if (interceptor != null)
            options.AddInterceptors(interceptor);

        return options;
    }
}
