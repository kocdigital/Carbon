using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.ElasticSearch.Abstractions
{
    public interface IElasticRepository<T> : IElasticReadOnlyRepository<T> where T : class
    {
        void DeleteById(string id, bool? refresh = null);
        DeleteResponse DeleteByIdAndReturn(string id, bool? refresh = null);
        Task DeleteByIdAsync(string id, bool? refresh = null);
        Task<DeleteResponse> DeleteByIdAndReturnAsync(string id, bool? refresh = null);
        void Add(T item);
        IndexResponse AddAndReturn(T item);
        Task AddAsync(T item, bool? refresh = null);
        Task<IndexResponse> AddAndReturnAsync(T item, bool? refresh = null);
        void Update(T item, bool? refresh = null);
        UpdateResponse<T> UpdateAndReturn(T item, bool? refresh = null);
        Task UpdateAsync(T item, bool? refresh = null);
        Task<UpdateResponse<T>> UpdateAndReturnAsync(T item, bool? refresh = null);
    }
}
