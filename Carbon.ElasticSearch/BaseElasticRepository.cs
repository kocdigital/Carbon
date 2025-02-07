using Nest;
using Carbon.ElasticSearch.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Carbon.Common.Constant;

namespace Carbon.ElasticSearch
{
    /// <inheritdoc cref="IElasticRepository{T}"/>
    public abstract class BaseElasticRepository<T> : BaseElasticReadOnlyRepository<T>, IElasticRepository<T> where T : class
    {
        private readonly bool _forceRefresh;

        protected BaseElasticRepository(IElasticSettings elasticSettings) : base(elasticSettings)
        {
            _forceRefresh = _elasticSettings != null && _elasticSettings.ForceRefresh;
        }
        public void Add(T item)
        {
            var response = _client.Index(item, i => i.Index(Index));
        }
        public IndexResponse AddAndReturn(T item)
        {
            return _client.Index(item, i => i.Index(Index));
        }
        public BulkResponse AddBulkAndReturn(IEnumerable<T> items)
        {
            return _client.Bulk(i => i.Index(Index).IndexMany(items));
        }
        public async Task AddAsync(T item, bool? refresh = null)
        {
            var response = await _client.IndexAsync(item, i => i.Index(Index)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task<IndexResponse> AddAndReturnAsync(T item, bool? refresh = null)
        {
            return await _client.IndexAsync(item, i => i.Index(Index)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task<BulkResponse> AddBulkAndReturnAsync(IEnumerable<T> items,CancellationToken cancellationToken = default)
        {
            return await _client.BulkAsync(b => b.Index(Index).IndexMany(items),cancellationToken);
        }
        public async Task<BulkResponse> AddBulkAndReturnAsync(IEnumerable<T> items, CancellationToken cancellationToken = default, bool? refresh = null)
        {
            return await _client.BulkAsync(b => b.Index(Index).IndexMany(items)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False), cancellationToken);
        }
        public void DeleteById(string id, bool? refresh = null)
        {
            var response = _client.Delete<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public DeleteResponse DeleteByIdAndReturn(string id, bool? refresh = null)
        {
            return _client.Delete<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task DeleteByIdAsync(string id, bool? refresh = null)
        {
            var response = await _client.DeleteAsync<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task<DeleteResponse> DeleteByIdAndReturnAsync(string id, bool? refresh = null)
        {
            return await _client.DeleteAsync<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task<DeleteByQueryResponse> DeleteAllAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            return await _client.DeleteByQueryAsync<T>(del => del
                    .Index(Index)
                    .Query(q => q.Term(t => t.Field(ElasticQueryConstant.KEYWORD_TENANTID).Value(tenantId)))
                    .Refresh()
                , cancellationToken);
        }

        public async Task UpdateAsync(T item, bool? refresh = null)
        {
            var response = await _client.UpdateAsync<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task<UpdateResponse<T>> UpdateAndReturnAsync(T item, bool? refresh = null)
        {
            return await _client.UpdateAsync<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public void Update(T item, bool? refresh = null)
        {
            var response = _client.Update<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public UpdateResponse<T> UpdateAndReturn(T item, bool? refresh = null)
        {
            return _client.Update<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }

        public BulkResponse BulkUpdate(IEnumerable<T> items, bool? refresh = null)
        {
           var bulkDescriptor = new BulkDescriptor();
            foreach (var item in items)
            {
                var documentId = GetDocumentId(item); 
                bulkDescriptor.Update<T>(u => u
                    .Index(Index)
                    .Id(documentId)  
                    .Doc(item)
                    .RetriesOnConflict(3)
                ).Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False);
            }

            var result =  _client.Bulk(bulkDescriptor);

            return result;
        }
        public async Task<BulkResponse> BulkUpdateAsync(IEnumerable<T> items, bool? refresh = null, CancellationToken cancellationToken = default)
        {
            var bulkDescriptor = new BulkDescriptor();
            foreach (var item in items)
            {
                var documentId = GetDocumentId(item); 
                bulkDescriptor.Update<T>(u => u
                    .Index(Index)
                    .Id(documentId) 
                    .Doc(item)
                    .RetriesOnConflict(3)
                ).Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False);
            }

            var result = await _client.BulkAsync(bulkDescriptor, cancellationToken);

            return result;

        }

        public string GetDocumentId<T>(T item)
        {
            var idProperty = item.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                var id = idProperty.GetValue(item)?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    return id;
                }
            }

            throw new InvalidOperationException("Item does not have a valid ID.");
        }
    }
}

