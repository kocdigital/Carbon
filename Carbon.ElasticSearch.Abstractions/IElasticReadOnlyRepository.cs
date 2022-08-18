using Carbon.Common;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.ElasticSearch.Abstractions
{
    public interface IElasticReadOnlyRepository<T> where T : class
    {
        Task<T> FindOneAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query);
        Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query);
        Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int size);
        Task<ISearchResponse<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null);
        Task<IList<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters);
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit);
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, string sortFieldName, bool? sortByDescending = null);
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, SortDescriptor<T> sort, int? offset, int? limit);
        Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit);
        Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, SortDescriptor<T> sort, int? offset, int? limit);
        Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, bool isHierarchical, int? offset, int? limit);
        Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, List<string> fieldNames, int? offset, int? limit);
        CountResponse Count(Func<CountDescriptor<T>, ICountRequest> selector = null);
        Task<CountResponse> CountAsync(Func<CountDescriptor<T>, ICountRequest> selector = null);

        #region SearchAsync
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor);
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables);
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive);
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive);
        #endregion

        #region SearchAfterAsync
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor);
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables);
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive);
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive);
        #endregion

        #region Scrolling
        Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor);
        Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, IList<Orderable> orderables);
        Task<ISearchResponse<T>> GetNextPageOfScrollAsync(string scrollId, string scrollTimeToLive);
        Task<ClearScrollResponse> ClearScrollAsync(string scrollId);
        #endregion

        Task<OpenPointInTimeResponse> OpenPointInTimeAsync(string pitTimeToLive);
        Task<ClosePointInTimeResponse> ClosePointInTimeAsync(string pitId);
    }
}
