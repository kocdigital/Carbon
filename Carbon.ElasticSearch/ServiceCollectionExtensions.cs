using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Carbon.ElasticSearch.Abstractions;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nest;
using System.Runtime;

namespace Carbon.ElasticSearch
{
    /// <summary>
	/// Contains extension methods for EliasticSearch like AddElasticSearchPersister etc. for <see cref="IServiceCollection"/>
	/// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
		/// From reading Configuration's "Elastic" key adds ElasticSearch to project.
		/// </summary>
		/// <param name="setupAction">
		/// Must contain code like this;
		/// <code>
		/// setupAction =>
		///	{
		///     setupAction.SetElasticConfiguration();
		///     setupAction.Build();
		/// }
		/// </code>
		/// </param>
		/// <remarks>
		/// Adds ElasticSearch Health Check to <see cref="IHealthChecksBuilder"/> by default
		/// </remarks>
        public static IServiceCollection AddElasticSearchPersister(this IServiceCollection services, IConfiguration configuration, Action<ElasticSettings> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            IElasticSettings elasticSettings = new ElasticSettings();
            configuration.GetSection("Elastic").Bind(elasticSettings);

            services.AddOptions();
            services.Configure(setupAction);
            setupAction?.Invoke(elasticSettings as ElasticSettings);
            services.AddSingleton(elasticSettings);


            AddElasticSearchPersisterHealthCheck(services, elasticSettings);
            return services;
        }
        /// <summary>
        /// Enables health check for ElasticSearch 
        /// </summary>
        /// <param name="failureStatus">Specifies for which state the system will say there is an error</param>
        public static IServiceCollection AddElasticSearchPersisterHealthCheck(this IServiceCollection services, IElasticSettings elasticSettings)
        {

            var healthCheck = services.AddHealthChecks()
                .AddAsyncCheck("elasticsearch-hc", async (cancellationToken) =>
            {
                var _elasticClient = new ElasticClient(elasticSettings.ConnectionSettings);

                var healthResponse = await _elasticClient.Cluster.HealthAsync();

                if (healthResponse.IsValid && healthResponse.Status == Elasticsearch.Net.Health.Green)
                {
                    return HealthCheckResult.Healthy("Elasticsearch cluster is healthy.");
                }
                else if (healthResponse.Status == Elasticsearch.Net.Health.Yellow)
                {
                    return HealthCheckResult.Degraded("Elasticsearch cluster is in a yellow state.");
                }
                else
                {
                    return HealthCheckResult.Unhealthy("Elasticsearch cluster is unhealthy.");
                }
            });
            return services;
        }
    }
}

