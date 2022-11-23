using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Carbon.ElasticSearch.Abstractions;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
        public static IServiceCollection AddElasticSearchPersisterHealthCheck(this IServiceCollection services, IElasticSettings settings, HealthStatus failureStatus = HealthStatus.Unhealthy)
        {
            var healthCheck = services.AddHealthChecks();
            foreach (var es in settings.Urls)
            {
                healthCheck = healthCheck.AddElasticsearch(x =>
                {
                    x.UseServer(es.AbsoluteUri);
                    x.UseBasicAuthentication(settings.UserName, settings.Password);
                }, $"/es-node[{es.Host}]", failureStatus, settings.Indexes.Select(x => x));
            }

            return services;
        }
    }
}

