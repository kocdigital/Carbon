using Nest;
using System;

namespace Carbon.ElasticSearch.Abstractions
{
    public class ElasticIndexMapping
    {
        public ElasticIndexMapping(string indexName, Func<CreateIndexDescriptor, ICreateIndexRequest> mapping)
        {
            IndexName = indexName;
            Mapping = mapping;
        }
        public string IndexName { get; }
        public Func<CreateIndexDescriptor, ICreateIndexRequest> Mapping { get; }
    }
}
