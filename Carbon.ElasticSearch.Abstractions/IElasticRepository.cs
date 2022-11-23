using Nest;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.ElasticSearch.Abstractions
{
    public interface IElasticRepository<T> : IElasticReadOnlyRepository<T> where T : class
    {
        /// <summary>
        /// Deletes record with given id
        /// </summary>
        /// <param name="id">id of a record to be deleted</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        void DeleteById(string id, bool? refresh = null);

        /// <summary>
        /// Deletes record with given id and returns ElastichSearch response
        /// </summary>
        /// <param name="id">id of a record to be deleted</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        DeleteResponse DeleteByIdAndReturn(string id, bool? refresh = null);

        /// <summary>
        /// Deletes record with given id
        /// </summary>
        /// <param name="id">id of a record to be deleted</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        Task DeleteByIdAsync(string id, bool? refresh = null);

        /// <summary>
        /// Deletes record with given id and returns record
        /// </summary>
        /// <param name="id">id of a record to be deleted</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        Task<DeleteResponse> DeleteByIdAndReturnAsync(string id, bool? refresh = null);

        /// <summary>
        /// Creates given record
        /// </summary>
        /// <param name="item">Record to be created</param>
        void Add(T item);

        /// <summary>
        /// Creates given record and returns ElastichSearch response
        /// </summary>
        /// <param name="item">Record to be created</param>
        IndexResponse AddAndReturn(T item);

        /// <summary>
        /// Creates given record
        /// </summary>
        /// <param name="item">Record to be created</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        Task AddAsync(T item, bool? refresh = null);

        /// <summary>
        /// Creates given record and returns ElastichSearch response
        /// </summary>
        /// <param name="item">Record to be created</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        Task<IndexResponse> AddAndReturnAsync(T item, bool? refresh = null);

        /// <summary>
        /// Updates given record
        /// </summary>
        /// <remarks> Uses _id field for matching items, for detail <see href="https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-update.html"/> </remarks>
        /// <param name="item">Record to be updated</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        void Update(T item, bool? refresh = null);

        /// <summary>
        /// Updates given record and returns ElastichSearch response
        /// </summary>
        /// <remarks> Uses _id field for matching items, for detail <see href="https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-update.html"/>  </remarks>
        /// <param name="item">Record to be updated</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        UpdateResponse<T> UpdateAndReturn(T item, bool? refresh = null);

        /// <summary>
        /// Updates given record
        /// </summary>
        /// <remarks>  Uses _id field for matching items, for detail <see href="https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-update.html"/> </remarks>
        /// <param name="item">Record to be updated</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        Task UpdateAsync(T item, bool? refresh = null);

        /// <summary>
        /// Updates given record and returns ElastichSearch response
        /// </summary>
        /// <remarks>  Uses _id field for matching items, for detail <see href="https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-update.html"/> </remarks>
        /// <param name="item">Record to be updated</param>
        /// <param name="refresh">if true; forces and waits for ElasticSearch to refresh the index after operation to make changes visible</param>
        Task<UpdateResponse<T>> UpdateAndReturnAsync(T item, bool? refresh = null);
    }
}
