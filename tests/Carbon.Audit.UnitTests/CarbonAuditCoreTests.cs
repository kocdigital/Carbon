using Carbon.Audit.Contracts;
using Carbon.Audit.Producer;
using Carbon.Audit.Producer.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Carbon.Audit.UnitTests;

public class CarbonAuditCoreTests
{
    [Fact]
    public void Core_assembly_references_no_MassTransit_assembly()
    {
        var referenced = typeof(AuditEvent).Assembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(n => n.StartsWith("MassTransit", StringComparison.OrdinalIgnoreCase)
                     || n.StartsWith("Carbon.MassTransit", StringComparison.OrdinalIgnoreCase)
                     || n.StartsWith("RabbitMQ", StringComparison.OrdinalIgnoreCase)
                     || n.StartsWith("GreenPipes", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.Empty(referenced);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddCarbonAudit_registers_context_and_interceptor_but_no_transport(bool enabled)
    {
        var configuration = TestHost.Configuration(enabled);
        var services = TestHost.Services(configuration);

        services.AddCarbonAudit(configuration);

        Assert.Contains(services, d => d.ServiceType == typeof(RequestContext));
        Assert.Contains(services, d => d.ServiceType == typeof(AuditInterceptor));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IAuditEventPublisher));
    }

    [Fact]
    public void UseCarbonAudit_throws_when_enabled_and_no_publisher_is_registered()
    {
        var configuration = TestHost.Configuration(auditEnabled: true);
        var services = TestHost.Services(configuration);
        services.AddCarbonAudit(configuration);
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        var ex = Assert.Throws<InvalidOperationException>(() => app.UseCarbonAudit());

        Assert.Contains("AddCarbonAuditMassTransit", ex.Message);
        Assert.Contains("Carbon.Audit.MassTransit7", ex.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(null)]
    public void UseCarbonAudit_is_a_no_op_when_disabled_or_unset(bool? enabled)
    {
        var configuration = TestHost.Configuration(enabled);
        var services = TestHost.Services(configuration);
        services.AddCarbonAudit(configuration);
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        var returned = app.UseCarbonAudit();

        Assert.Same(app, returned);
        var pipeline = app.Build();
        Assert.NotNull(pipeline);
    }

    [Fact]
    public void UseCarbonAudit_accepts_any_registered_publisher_when_enabled()
    {
        var configuration = TestHost.Configuration(auditEnabled: true);
        var services = TestHost.Services(configuration);
        services.AddCarbonAudit(configuration);
        services.AddScoped(_ => Mock.Of<IAuditEventPublisher>());
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        var ex = Record.Exception(() => app.UseCarbonAudit());

        Assert.Null(ex);
    }
}
