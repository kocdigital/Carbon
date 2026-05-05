using Carbon.Common.Constant;
using Carbon.ElasticSearch.Abstractions;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        private static readonly IndexResponse _emptyIndexResponse = new IndexResponse();
        private static readonly BulkResponse _emptyBulkResponse = new BulkResponse();
        private static readonly DeleteResponse _emptyDeleteResponse = new DeleteResponse();
        private static readonly DeleteByQueryResponse _emptyDeleteByQueryResponse = new DeleteByQueryResponse();
        private static readonly UpdateResponse<T> _emptyUpdateResponse = new UpdateResponse<T>();
        public void Add(T item)
        {
            if (!IsClientAvailable()) return;
            var response = _client.Index(item, i => i.Index(Index));
        }
        public IndexResponse AddAndReturn(T item)
        {
            if (!IsClientAvailable()) return _emptyIndexResponse;
            return _client.Index(item, i => i.Index(Index));
        }
        public BulkResponse AddBulkAndReturn(IEnumerable<T> items)
        {
            if (!IsClientAvailable()) return _emptyBulkResponse;
            return _client.Bulk(i => i.Index(Index).IndexMany(items));
        }
        public async Task AddAsync(T item, bool? refresh = null, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return;
            var response = await _client.IndexAsync(item, i => i.Index(Index)
                .Refresh((_forceRefresh || (refresh ?? false))
                    ? Elasticsearch.Net.Refresh.True
                    : Elasticsearch.Net.Refresh.False), cancellationToken);
        }
        public async Task<IndexResponse> AddAndReturnAsync(T item, bool? refresh = null, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return _emptyIndexResponse;
            return await _client.IndexAsync(item, i => i.Index(Index)
                .Refresh((_forceRefresh || (refresh ?? false))
                    ? Elasticsearch.Net.Refresh.True
                    : Elasticsearch.Net.Refresh.False), cancellationToken);
        }
        public async Task<BulkResponse> AddBulkAndReturnAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return _emptyBulkResponse;
            return await _client.BulkAsync(b => b.Index(Index).IndexMany(items), cancellationToken);
        }
        public async Task<BulkResponse> AddBulkAndReturnAsync(IEnumerable<T> items, CancellationToken cancellationToken = default, bool? refresh = null)
        {
            if (!IsClientAvailable()) return _emptyBulkResponse;
            return await _client.BulkAsync(b => b.Index(Index).IndexMany(items)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False), cancellationToken);
        }
        public void DeleteById(string id, bool? refresh = null)
        {
            if (!IsClientAvailable()) return;
            var response = _client.Delete<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public DeleteResponse DeleteByIdAndReturn(string id, bool? refresh = null)
        {
            if (!IsClientAvailable()) return _emptyDeleteResponse;
            return _client.Delete<T>(new DocumentPath<T>(id), x => x.Index(Index)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public async Task DeleteByIdAsync(string id, bool? refresh = null, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return;
            var response = await _client.DeleteAsync<T>(new DocumentPath<T>(id), x => x.Index(Index)
                .Refresh((_forceRefresh || (refresh ?? false))
                    ? Elasticsearch.Net.Refresh.True
                    : Elasticsearch.Net.Refresh.False), cancellationToken);
        }
        public async Task<DeleteResponse> DeleteByIdAndReturnAsync(string id, bool? refresh = null, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return _emptyDeleteResponse;
            return await _client.DeleteAsync<T>(new DocumentPath<T>(id), x => x.Index(Index)
                .Refresh((_forceRefresh || (refresh ?? false))
                    ? Elasticsearch.Net.Refresh.True
                    : Elasticsearch.Net.Refresh.False), cancellationToken);
        }
        public async Task<DeleteByQueryResponse> DeleteAllAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return _emptyDeleteByQueryResponse;
            return await _client.DeleteByQueryAsync<T>(del => del
                    .Index(Index)
                    .Query(q => q.Term(t => t.Field(ElasticQueryConstant.KEYWORD_TENANTID).Value(tenantId)))
                    .Refresh()
                , cancellationToken);
        }

        public async Task UpdateAsync(T item, bool? refresh = null, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return;
            var response = await _client.UpdateAsync<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
                .Refresh((_forceRefresh || (refresh ?? false))
                    ? Elasticsearch.Net.Refresh.True
                    : Elasticsearch.Net.Refresh.False), cancellationToken);
        }
        public async Task<UpdateResponse<T>> UpdateAndReturnAsync(T item, bool? refresh = null, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return _emptyUpdateResponse;
            return await _client.UpdateAsync<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
                .Refresh((_forceRefresh || (refresh ?? false))
                    ? Elasticsearch.Net.Refresh.True
                    : Elasticsearch.Net.Refresh.False), cancellationToken);
        }
        public void Update(T item, bool? refresh = null)
        {
            if (!IsClientAvailable()) return;
            var response = _client.Update<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }
        public UpdateResponse<T> UpdateAndReturn(T item, bool? refresh = null)
        {
            if (!IsClientAvailable()) return _emptyUpdateResponse;
            return _client.Update<T>(item, u => u.Index(Index).Doc(item).RetryOnConflict(3)
            .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }

        public BulkResponse BulkUpdate(IEnumerable<T> items, bool? refresh = null)
        {
            if (!IsClientAvailable()) return _emptyBulkResponse;
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
            if (!IsClientAvailable()) return _emptyBulkResponse;
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
        public UpdateResponse<T> UpdateWithFieldRemoval(T item, string[] fieldsToRemove, bool? refresh = null)
        {
            if (!IsClientAvailable()) return _emptyUpdateResponse;
            var documentId = GetDocumentId(item);
            if (fieldsToRemove == null || fieldsToRemove.Length == 0)
                throw new ArgumentException("fieldsToRemove must not be null or empty.", nameof(fieldsToRemove));

            var scriptSource = string.Join(" ", fieldsToRemove.Select(f => $"ctx._source.remove('{f}');"));

            return _client.Update<T>(documentId, u => u
                .Index(Index)
                .Script(s => s.Source(scriptSource))
                .RetryOnConflict(3)
                .Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False));
        }

        public async Task<UpdateResponse<T>> UpdateWithFieldRemovalAsync(T item, string[] fieldsToRemove, bool? refresh = null, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return _emptyUpdateResponse;
            var documentId = GetDocumentId(item);
            if (fieldsToRemove == null || fieldsToRemove.Length == 0)
                throw new ArgumentException("fieldsToRemove must not be null or empty.", nameof(fieldsToRemove));

            var scriptSource = string.Join(" ", fieldsToRemove.Select(f => $"ctx._source.remove('{f}');"));

            return await _client.UpdateAsync<T>(documentId, u => u
                .Index(Index)
                .Script(s => s.Source(scriptSource))
                .RetryOnConflict(3)
                .Refresh((_forceRefresh || (refresh ?? false))
                    ? Elasticsearch.Net.Refresh.True
                    : Elasticsearch.Net.Refresh.False), cancellationToken);
        }

        public BulkResponse BulkUpdateWithFieldRemoval(IEnumerable<T> items, string[] fieldsToRemove, bool? refresh = null)
        {
            if (!IsClientAvailable()) return _emptyBulkResponse;
            if (fieldsToRemove == null || fieldsToRemove.Length == 0)
                throw new ArgumentException("fieldsToRemove must not be null or empty.", nameof(fieldsToRemove));

            var scriptSource = string.Join(" ", fieldsToRemove.Select(f => $"ctx._source.remove('{f}');"));
            var bulkDescriptor = new BulkDescriptor();

            foreach (var item in items)
            {
                var documentId = GetDocumentId(item);
                bulkDescriptor.Update<T>(u => u
                    .Index(Index)
                    .Id(documentId)
                    .Script(s => s.Source(scriptSource))
                    .RetriesOnConflict(3)
                ).Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False);
            }

            return _client.Bulk(bulkDescriptor);
        }

        public async Task<BulkResponse> BulkUpdateWithFieldRemovalAsync(IEnumerable<T> items, string[] fieldsToRemove, bool? refresh = null, CancellationToken cancellationToken = default)
        {
            if (!IsClientAvailable()) return _emptyBulkResponse;
            if (fieldsToRemove == null || fieldsToRemove.Length == 0)
                throw new ArgumentException("fieldsToRemove must not be null or empty.", nameof(fieldsToRemove));

            var scriptSource = string.Join(" ", fieldsToRemove.Select(f => $"ctx._source.remove('{f}');"));
            var bulkDescriptor = new BulkDescriptor();

            foreach (var item in items)
            {
                var documentId = GetDocumentId(item);
                bulkDescriptor.Update<T>(u => u
                    .Index(Index)
                    .Id(documentId)
                    .Script(s => s.Source(scriptSource))
                    .RetriesOnConflict(3)
                ).Refresh((_forceRefresh || (refresh ?? false)) ? Elasticsearch.Net.Refresh.True : Elasticsearch.Net.Refresh.False);
            }

            return await _client.BulkAsync(bulkDescriptor, cancellationToken);
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

