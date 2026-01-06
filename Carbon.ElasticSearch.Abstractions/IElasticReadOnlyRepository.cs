using Carbon.Common;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.ElasticSearch.Abstractions
{
    public interface IElasticReadOnlyRepository<T> where T : class
    {
        /// <summary>
		/// Returns one record for given query
        /// </summary>
        /// <param name="query">Nest query <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html"/></param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<T> FindOneAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, CancellationToken cancellationToken = default);
        
        /// <summary>
		/// Returns recors found for given query. It's restricted with 1000 items
        /// </summary>
        /// <param name="query">Nest query <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html"/></param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
		Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, CancellationToken cancellationToken = default);
        
        /// <summary>
		/// Returns recors found for given query limited by size param
        /// </summary>
        /// <param name="query">Nest query <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html"/></param>
        /// <param name="size">Count of to be returned results. If it's 0 then changed as 1000 on the code</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
		Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int size, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Returns filtered records for given filters. Uses Elastich Search's "Filter" for filtering. 
        /// </summary>
        /// <param name="filters">List of Nest queries to filter <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html#combining-queries"/></param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<IList<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns filtered records for given filters with offset and limit.
        /// </summary>
        /// <param name="filters">List of Nest queries to filter <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html#combining-queries"/></param>
        /// <param name="offset">Starting point as a count for filter</param>
        /// <param name="limit">Records should be returned up to a maximum limit.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Returns filtered records for given filters with offset, limit, and sorting.
        /// </summary>
        /// <param name="filters">List of Nest queries to filter</param>
        /// <param name="offset">Starting point as a count for filter</param>
        /// <param name="limit">Records should be returned up to a maximum limit.</param>
        /// <param name="sortFieldName">Sorting field name</param>
        /// <param name="sortByDescending">Sorting direction</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, string sortFieldName, bool? sortByDescending = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Returns filtered records for given filters with offset, limit, and sort descriptor.
        /// </summary>
        /// <param name="filters">List of Nest queries to filter</param>
        /// <param name="sort">Sort query <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/sort-usage.html#sort-usage"/></param>
        /// <param name="offset">Starting point as a count for filter</param>
        /// <param name="limit">Records should be returned up to a maximum limit.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, SortDescriptor<T> sort, int? offset, int? limit, CancellationToken cancellationToken = default);
        
        //TODO: Remove this, it's not related to Carbon
        /// <summary>
        /// Returns filtered discovery records for given filters with offset and limit.
        /// </summary>
        /// <param name="filters">List of Nest queries to filter</param>
        /// <param name="offset">Starting point as a count for filter</param>
        /// <param name="limit">Records should be returned up to a maximum limit.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, CancellationToken cancellationToken = default);
        
        //TODO: Remove this, it's not related to Carbon
        /// <summary>
        /// Returns filtered discovery records for given filters with sort descriptor, offset, and limit.
        /// </summary>
        /// <param name="filters">List of Nest queries to filter</param>
        /// <param name="sort">Sort query for sorting. <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/sort-usage.html#sort-usage"/></param>
        /// <param name="offset">Starting point as a count for filter</param>
        /// <param name="limit">Records should be returned up to a maximum limit.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, SortDescriptor<T> sort, int? offset, int? limit, CancellationToken cancellationToken = default);
        
        //TODO: Remove this, it's not related to Carbon
        /// <summary>
        /// Returns discovered URLs for given filters with hierarchical option, offset, and limit.
        /// </summary>
        /// <param name="filters">List of Nest queries to filter</param>
        /// <param name="isHierarchical">Indicates if the result should be hierarchical</param>
        /// <param name="offset">Starting point as a count for filter</param>
        /// <param name="limit">Records should be returned up to a maximum limit.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, bool isHierarchical, int? offset, int? limit, CancellationToken cancellationToken = default);
       
        //TODO: Remove this, it's not related to Carbon
        /// <summary>
        /// Returns discovered URLs for given filters with field names, offset, and limit.
        /// </summary>
        /// <param name="filters">List of Nest queries to filter</param>
        /// <param name="fieldNames">List of field names to include in the result</param>
        /// <param name="offset">Starting point as a count for filter</param>
        /// <param name="limit">Records should be returned up to a maximum limit.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, List<string> fieldNames, int? offset, int? limit, CancellationToken cancellationToken = default);
        
        /// <summary>
		/// Returns Count for given query
        /// </summary>
        /// <param name="selector">Count request, for detail <see href="https://www.elastic.co/guide/en/elasticsearch/reference/master/search-count.html"/></param>
        CountResponse Count(Func<CountDescriptor<T>, ICountRequest> selector = null);
        
        /// <summary>
		/// Returns Count for given query
        /// </summary>
        /// <param name="selector">Count request, for detail <see href="https://www.elastic.co/guide/en/elasticsearch/reference/master/search-count.html"/></param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
		Task<CountResponse> CountAsync(Func<CountDescriptor<T>, ICountRequest> selector = null, CancellationToken cancellationToken = default);

        #region SearchAsync
        /// <summary>
        /// Returns search response <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html#_search_response"/>
        /// </summary>
        /// <param name="selector">Nest search request <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/reference-search.html"/></param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Sort and search the elastic index using a query and sorting descriptors  
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search. 
        /// <para><see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/sort-usage.html#sort-usage"/></para></param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access 
        /// further items instead.</remarks>
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sort and search the elastic index using a query and orderables 
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access 
        /// further items instead.</remarks>
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sort and search the elastic index using a query, sorting descriptors, and point in time.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search.</param>
        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// <para>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access further items instead. </para>
        /// <para>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</para>
        /// </remarks>
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sort and search the elastic index using a query, orderables, and point in time.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// <para>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access further items instead.</para>
        /// <para>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</para>
        /// </remarks>
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive, CancellationToken cancellationToken = default);
        #endregion

        #region SearchAfterAsync
        /// <summary>
        /// Sort and search the elastic index, using a query, sorting descriptors, and a starting item
        /// </summary>
        /// <param name="query">Function that will provide the query for the search. For detailed info about see: <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html#combining-queries"/></param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search. For detailed info about see: <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/sort-usage.html#sort-usage"/></param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sort and search the elastic index, using a query, orderables, and a starting item
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sort and search the elastic index, using a query, sorting descriptors, a starting item, and point in time.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search.</param>
        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</remarks>
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sort and search the elastic index, using a query, orderables, a starting item, and point in time.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</remarks>
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive, CancellationToken cancellationToken = default);
        #endregion

        #region Scrolling
        /// <summary>
        /// Sort and search the elastic index using a query and a sorting descriptor, and then create a scrolling window to access further results ion the future.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="scrollTimeToLive">Amount of time the scroll window instance will be retained after the search</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Sort and search the elastic index using a query and orderables, and then create a scrolling window to access further results ion the future.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="scrollTimeToLive">Amount of time the scroll window instance will be retained after the search</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, IList<Orderable> orderables, CancellationToken cancellationToken = default);
        /// <summary>
        /// Get the next batch of search results from a scrolling window
        /// </summary>
        /// <param name="scrollId">The Id of the scrolling window created by a previous search</param>
        /// <param name="scrollTimeToLive">Amount of time the scroll window instance will be retained after the search</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<ISearchResponse<T>> GetNextPageOfScrollAsync(string scrollId, string scrollTimeToLive, CancellationToken cancellationToken = default);
        /// <summary>
        /// Destroy the scrolling window instance
        /// </summary>
        /// <param name="scrollId">The Id of the scrolling window to be destroyed.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>It is recommended to destroy the scrolling window as soon as it is no longer needed, to free up memory.</remarks>
        Task<ClearScrollResponse> ClearScrollAsync(string scrollId, CancellationToken cancellationToken = default);
        #endregion

        /// <summary>
        /// Creates and returns a new "point in time" instance.
        /// </summary>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</remarks>
        Task<OpenPointInTimeResponse> OpenPointInTimeAsync(string pitTimeToLive, CancellationToken cancellationToken = default);

        /// <summary>
        /// Destroy the point in time instance
        /// </summary>
        /// <param name="pitId">The Id of the point in time instance to be destroyed.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>It is recommended to destroy the instances as soon as they are no longer needed, to free up memory.
        /// <para>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</para>
        /// </remarks>
        Task<ClosePointInTimeResponse> ClosePointInTimeAsync(string pitId, CancellationToken cancellationToken = default);
    }
}
