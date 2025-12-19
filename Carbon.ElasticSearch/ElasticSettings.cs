using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using Carbon.ElasticSearch.Abstractions;

namespace Carbon.ElasticSearch
{
    /// <summary>
	/// <inheritdoc cref="IElasticSettings"/>
	/// </summary>
    public class ElasticSettings : IElasticSettings, IOptions<ElasticSettings>
    {
        public ElasticSettings()
        {
            Indexes = new List<string>();
        }
        public ElasticSettings Value => this;

        public Uri[] Urls { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; } 
		public bool ForceRefresh { get; set; }

		public ConnectionSettings ConnectionSettings { get; set; }
        protected IList<ElasticIndexMapping> Mappings { get; set; }
        public List<string> Indexes { get; set; }

		public void SetIndexsAndAutoMappings(params ElasticIndexMapping[] mappings)
        {
            Mappings = mappings;

        }
        public void Build()
        {
            IElasticSettings settings = this as IElasticSettings;

            var connectionPool = new StaticConnectionPool(settings.Urls);
            ConnectionSettings = new ConnectionSettings(connectionPool);

            ConnectionSettings.DisablePing();
            ConnectionSettings.EnableDebugMode();
            ConnectionSettings.BasicAuthentication(settings.UserName, settings.Password)
                               .PrettyJson(false)
                              .EnableHttpCompression()
                              .DisableDirectStreaming(true)
                              .RequestTimeout(TimeSpan.FromSeconds(settings.Timeout));

            var client = new ElasticClient(ConnectionSettings);

            foreach (var map in Mappings)
            {
                if (!string.IsNullOrEmpty(map.IndexName) && !client.Indices.ExistsAsync(map.IndexName).Result.Exists)
                {
                    var result = client.Indices.Create(map.IndexName, map.Mapping);

                    if (!result.IsValid)
                    {
                        throw new ElasticsearchClientException("ElasticIndex cannot be created!" + Environment.NewLine + result.OriginalException);
                    }
                }
            }


        }

    }
}

