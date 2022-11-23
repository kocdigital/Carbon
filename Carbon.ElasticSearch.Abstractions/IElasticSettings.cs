using Nest;
using System;
using System.Collections.Generic;

namespace Carbon.ElasticSearch.Abstractions
{
    /// <summary>
	/// Settings for connecting Elastic Search Cluster
	/// </summary>
    public interface IElasticSettings
    {
        #region Properties
        /// <summary>
		/// Cluster urls, can be single too
		/// </summary>
        Uri[] Urls { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        int Timeout { get; set; }
        List<string> Indexes { get; set; }
        ConnectionSettings ConnectionSettings { get; set; }
        /// <summary>
		/// Forces elastic search to refresh after Create/Update/Delte operation. <see href="https://www.elastic.co/guide/en/elasticsearch/reference/7.17/indices-refresh.html"/>
		/// </summary>
        bool ForceRefresh { get; set; }

        #endregion

        #region Methods
        /// <summary>
		/// Used for injecting Mappings for Indexes, <see cref="Build"/> should take those mappings and create indices.
		/// </summary>
		/// <param name="mappings"></param>
        void SetIndexsAndAutoMappings(params ElasticIndexMapping[] mappings);
        /// <summary>
		/// Should create indices using mappings given by <see cref="SetIndexsAndAutoMappings"/>
		/// </summary>
        void Build();
        #endregion
    }
}
