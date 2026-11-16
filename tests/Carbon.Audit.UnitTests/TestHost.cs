using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Carbon.Audit.UnitTests;

internal static class TestHost
{
    public static IConfiguration Configuration(bool? auditEnabled)
    {
        var values = new Dictionary<string, string?>
        {
            ["MassTransit:BusType"] = "RabbitMQ",
            ["MassTransit:RabbitMq:Host"] = "localhost",
            ["MassTransit:RabbitMq:Port"] = "5672",
            ["MassTransit:RabbitMq:VirtualHost"] = "/",
            ["MassTransit:RabbitMq:Username"] = "guest",
            ["MassTransit:RabbitMq:Password"] = "guest",
        };
        if (auditEnabled.HasValue)
            values["CarbonAudit:Enabled"] = auditEnabled.Value ? "true" : "false";

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    public static ServiceCollection Services(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        return services;
    }
}
