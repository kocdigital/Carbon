using Carbon.Audit.Bus;
using Carbon.Audit.Producer;
using Carbon.Audit.Producer.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Carbon.Audit.UnitTests;

public class CarbonAuditRegistrationTests
{
    [Fact]
    public void Carbon_Audit_is_built_against_MassTransit_7()
    {
        var massTransit = typeof(IAuditBus).Assembly.GetReferencedAssemblies()
            .Where(a => a.Name is "MassTransit" or "MassTransit.Abstractions")
            .ToList();

        Assert.NotEmpty(massTransit);
        Assert.All(massTransit, a => Assert.Equal(7, a.Version!.Major));
        Assert.DoesNotContain(massTransit, a => a.Name == "MassTransit.Abstractions");
    }

    [Fact]
    public void AddCarbonAudit_registers_bus_and_publisher_when_enabled()
    {
        var configuration = TestHost.Configuration(auditEnabled: true);
        var services = TestHost.Services(configuration);

        services.AddCarbonAudit(configuration);

        Assert.Contains(services, d => d.ServiceType == typeof(RequestContext));
        Assert.Contains(services, d => d.ServiceType == typeof(AuditInterceptor));
        Assert.Contains(services, d => d.ServiceType == typeof(IAuditBus));
        var publisher = Assert.Single(services, d => d.ServiceType == typeof(IAuditEventPublisher));
        Assert.Equal(ServiceLifetime.Scoped, publisher.Lifetime);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(null)]
    public void AddCarbonAudit_registers_no_bus_when_disabled_or_unset(bool? enabled)
    {
        var configuration = TestHost.Configuration(enabled);
        var services = TestHost.Services(configuration);

        services.AddCarbonAudit(configuration);

        Assert.Contains(services, d => d.ServiceType == typeof(AuditInterceptor));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IAuditBus));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IAuditEventPublisher));
    }

    [Fact]
    public void Publisher_resolves_without_starting_the_bus()
    {
        var configuration = TestHost.Configuration(auditEnabled: true);
        var services = TestHost.Services(configuration);
        services.AddCarbonAudit(configuration);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var publisher = scope.ServiceProvider.GetRequiredService<IAuditEventPublisher>();

        Assert.IsType<RabbitMqAuditEventPublisher>(publisher);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UseCarbonAudit_builds_the_pipeline(bool enabled)
    {
        var configuration = TestHost.Configuration(enabled);
        var services = TestHost.Services(configuration);
        services.AddCarbonAudit(configuration);
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        var ex = Record.Exception(() => app.UseCarbonAudit().Build());

        Assert.Null(ex);
    }
}
