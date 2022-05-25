using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.ElasticSearch.Abstractions
{
    public interface IElasticRepository<T> where T : class
    {
        Task<T> FindOneAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query);
        Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query);
        Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int size);
        Task<ISearchResponse<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null);
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
        Task<IList<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters);
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit);
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, string sortFieldName, bool? sortByDescending = null);
        Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit);
        Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, bool isHierarchical, int? offset, int? limit);
        Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, List<string> fieldNames, int? offset, int? limit);
        CountResponse Count(Func<CountDescriptor<T>, ICountRequest> selector = null);
        Task<CountResponse> CountAsync(Func<CountDescriptor<T>, ICountRequest> selector = null);
    }
}
