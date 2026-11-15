using Carbon.Audit.Bus;
using Carbon.Audit.Producer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Carbon.Audit.UnitTests;

// Compiled into both Carbon.Audit.UnitTests (MassTransit 8) and
// Carbon.Audit.MassTransit7.UnitTests (MassTransit 7).
public class AuditMassTransitTransportTests
{
#if CARBON_MASSTRANSIT7
    private const string ExpectedMassTransitMajor = "7";
#else
    private const string ExpectedMassTransitMajor = "8";
#endif

    [Fact]
    public void Transport_is_built_against_the_expected_MassTransit_major()
    {
        var massTransit = typeof(IAuditBus).Assembly.GetReferencedAssemblies()
            .Where(a => a.Name == "MassTransit" || a.Name == "MassTransit.Abstractions")
            .ToList();

        Assert.NotEmpty(massTransit);
        Assert.All(massTransit, a => Assert.Equal(ExpectedMassTransitMajor, a.Version!.Major.ToString()));
    }

    [Fact]
    public void AddCarbonAuditMassTransit_registers_nothing_when_disabled()
    {
        var configuration = TestHost.Configuration(auditEnabled: false);
        var services = TestHost.Services(configuration);
        var before = services.Count;

        services.AddCarbonAuditMassTransit(configuration);

        Assert.Equal(before, services.Count);
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IAuditBus));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IAuditEventPublisher));
    }

    [Fact]
    public void AddCarbonAuditMassTransit_registers_bus_and_publisher_when_enabled()
    {
        var configuration = TestHost.Configuration(auditEnabled: true);
        var services = TestHost.Services(configuration);

        services.AddCarbonAuditMassTransit(configuration);

        Assert.Contains(services, d => d.ServiceType == typeof(IAuditBus));
        var publisher = Assert.Single(services, d => d.ServiceType == typeof(IAuditEventPublisher));
        Assert.Equal(ServiceLifetime.Scoped, publisher.Lifetime);
    }

    [Fact]
    public void AddCarbonAuditMassTransit_is_idempotent()
    {
        var configuration = TestHost.Configuration(auditEnabled: true);
        var services = TestHost.Services(configuration);

        services.AddCarbonAuditMassTransit(configuration);
        var afterFirst = services.Count;
        services.AddCarbonAuditMassTransit(configuration);

        Assert.Equal(afterFirst, services.Count);
    }

    [Fact]
    public void Core_plus_transport_passes_the_startup_check_when_enabled()
    {
        var configuration = TestHost.Configuration(auditEnabled: true);
        var services = TestHost.Services(configuration);
        services.AddCarbonAudit(configuration);
        services.AddCarbonAuditMassTransit(configuration);
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        var ex = Record.Exception(() => app.UseCarbonAudit());

        Assert.Null(ex);
    }

    [Fact]
    public void Publisher_resolves_without_starting_the_bus()
    {
        var configuration = TestHost.Configuration(auditEnabled: true);
        var services = TestHost.Services(configuration);
        services.AddCarbonAudit(configuration);
        services.AddCarbonAuditMassTransit(configuration);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var publisher = scope.ServiceProvider.GetRequiredService<IAuditEventPublisher>();

        Assert.IsType<RabbitMqAuditEventPublisher>(publisher);
    }

    [Fact]
    public void AddCarbonAuditMassTransit_keeps_a_service_provided_publisher_and_starts_no_bus()
    {
        var configuration = TestHost.Configuration(auditEnabled: true);
        var services = TestHost.Services(configuration);
        var custom = Moq.Mock.Of<IAuditEventPublisher>();
        services.AddScoped(_ => custom);

        services.AddCarbonAuditMassTransit(configuration);

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IAuditBus));
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.Same(custom, scope.ServiceProvider.GetRequiredService<IAuditEventPublisher>());
    }
}
