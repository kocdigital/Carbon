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
        Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int size)
        Task<ISearchResponse<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null);
        void DeleteById(string id);
        DeleteResponse DeleteByIdAndReturn(string id);
        Task DeleteByIdAsync(string id);
        Task<DeleteResponse> DeleteByIdAndReturnAsync(string id);
        void Add(T item);
        IndexResponse AddAndReturn(T item);
        Task AddAsync(T item);
        Task<IndexResponse> AddAndReturnAsync(T item);
        void Update(T item);
        UpdateResponse<T> UpdateAndReturn(T item);
        Task UpdateAsync(T item);
        Task<UpdateResponse<T>> UpdateAndReturnAsync(T item);
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
