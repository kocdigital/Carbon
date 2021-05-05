using Nest;
using Carbon.ElasticSearch.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carbon.ElasticSearch
{

    public abstract class BaseElasticRepository<T> : IElasticRepository<T> where T : class
    {
        private readonly ElasticClient _client;
        private readonly IElasticSettings _elasticSettings;
        public abstract string Index { get; }

        protected BaseElasticRepository(IElasticSettings elasticSettings)
        {
            _client = new ElasticClient(elasticSettings.ConnectionSettings);
            _elasticSettings = elasticSettings;
        }
        public void Add(T item)
        {
            var response = _client.Index(item, i => i.Index(Index));
        }
        public IndexResponse AddAndReturn(T item)
        {
            return _client.Index(item, i => i.Index(Index));
        }
        public async Task AddAsync(T item)
        {
            var response = await _client.IndexAsync(item, i => i.Index(Index)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task<IndexResponse> AddAndReturnAsync(T item)
        {
            return await _client.IndexAsync(item, i => i.Index(Index)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public void DeleteById(string id)
        {
            var response = _client.Delete<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public DeleteResponse DeleteByIdAndReturn(string id)
        {
            return _client.Delete<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task DeleteByIdAsync(string id)
        {
            var response = await _client.DeleteAsync<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task<DeleteResponse> DeleteByIdAndReturnAsync(string id)
        {
            return await _client.DeleteAsync<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }

        public async Task<T> FindOneAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(0).Size(1).Query(query));

            return response.Documents.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(0).Size(1000).Query(query));

            return response.Documents.ToList();
        }

        public async Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int size)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(0).Size(size > 0 ? size : 1000).Query(query));

            return response.Documents.ToList();
        }

        public async Task UpdateAsync(T item)
        {
            var response = await _client.UpdateAsync<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task<UpdateResponse<T>> UpdateAndReturnAsync(T item)
        {
            return await _client.UpdateAsync<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public void Update(T item)
        {
            var response = _client.Update<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public UpdateResponse<T> UpdateAndReturn(T item)
        {
            return _client.Update<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_elasticSettings != null && _elasticSettings.ForceRefresh) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task<ISearchResponse<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null)
        {
            return await _client.SearchAsync<T>(selector);
        }
        public async Task<IList<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).Size(1000).Query(q => q.Bool(bq => bq.Filter(filters.ToArray()))));
            return response.Documents.ToList();
        }
        public async Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(offset).Size(limit).Query(q => q.Bool(bq => bq.Filter(filters.ToArray()))));
            return response;
        }

        public async Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, string sortFieldName, bool? sortByDescending = null)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .From(offset)
                .Size(limit)
                .Query(q => q.Bool(bq => bq.Filter(filters.ToArray())))
                .Sort(s => sortByDescending ?? false ? s.Descending($"{sortFieldName}.keyword") : s.Ascending($"{sortFieldName}.keyword")));
            return response;
        }

        public async Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(offset).Size(limit).Query(q => q.Bool(bq => bq.Filter(filters.ToArray()))));
            return response;
        }
        public async Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, bool isHierarchical, int? offset, int? limit)
        {
            var field = isHierarchical ? "hierarchicalUrl" : "id";
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(offset).Size(limit).Source(sr => sr.Includes(fi => fi.Field(field))).Query(q => q.Bool(bq => bq.Filter(filters.ToArray()))));
            return response;
        }
        public async Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, List<string> fieldNames, int? offset, int? limit)
        {
            var fieldList = new List<Field>();
            foreach (var fieldName in fieldNames)
            {
                fieldList.Add(new Field(fieldName));
            }
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(offset).Size(limit).Source(sr => sr.Includes(fi => fi.Fields(fieldList)))
            .Query(q => q.Bool(bq => bq.Filter(filters.ToArray()))));
            return response;
        }

        public CountResponse Count(Func<CountDescriptor<T>, ICountRequest> selector = null)
        {
            return _client.Count<T>(selector);
        }
        public async Task<CountResponse> CountAsync(Func<CountDescriptor<T>, ICountRequest> selector = null)
        {
            return await _client.CountAsync<T>(selector);
        }
    }
}

