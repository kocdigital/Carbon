using Nest;
using System;
using System.Collections.Generic;

namespace Carbon.ElasticSearch.Abstractions
{
    public interface IElasticSettings
    {
        #region Properties
        Uri[] Urls { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        int Timeout { get; set; }
        List<string> Indexes { get; set; }
        ConnectionSettings ConnectionSettings { get; set; }
        bool ForceRefresh { get; set; }

        #endregion

        #region Methods
        void SetIndexsAndAutoMappings(params ElasticIndexMapping[] mappings);
        void Build();
        #endregion
    }
}
