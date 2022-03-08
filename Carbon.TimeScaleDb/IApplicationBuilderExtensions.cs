using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace Carbon.TimeScaleDb
{
    public static class IApplicationBuilderExtensions
    {

        public static void AddTimeScaleDatabaseContextHealthCheck(this IServiceCollection services, string connectionString, HealthStatus failureStatus = HealthStatus.Unhealthy)
        {
            services.AddHealthChecks().AddNpgSql(connectionString, failureStatus: failureStatus, name: $"TimeScaleDb");
        }
    }
}
