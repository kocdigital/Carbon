using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Carbon.ElasticSearch.Abstractions;

namespace Carbon.ElasticSearch
{
    public static class ServiceCollectionExtensions
    {
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

            return services;
        }
    }
}

