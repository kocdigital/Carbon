using Carbon.Common;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.ElasticSearch.Abstractions
{
    public interface IElasticReadOnlyRepository<T> where T : class
    {
        /// <summary>
		/// Returns one record for given query
		/// </summary>
		/// <remarks> <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html"/>  </remarks>
		/// <param name="query">Nest query <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html"/></param>
        Task<T> FindOneAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query);
        
        /// <summary>
		/// Returns recors found for given query. It's restricted with 1000 items
		/// </summary>
		/// <remarks> <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html"/> </remarks>
		/// <param name="query">Nest query <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html"/></param>
		Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query);
        
        /// <summary>
		/// Returns recors found for given query limited by size param
		/// </summary>
		/// <remarks>  <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html"/> </remarks>
		/// <param name="query">Nest query <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html"/></param>
		/// <param name="size">Count of to be returned results. If it's 0 then changed as 1000 on the code</param>
		Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int size);
        
        /// <summary>
        /// Returns filtered records for given filters. Uses Elastich Search's "Filter" for filtering. 
        /// </summary>
        /// <remarks> 
        /// <see href="https://www.elastic.co/guide/en/elasticsearch/reference/7.17/query-filter-context.html#filter-context"/>
        /// <para><seealso href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html#combining-queries"/> </para>
        /// </remarks>
        /// <param name="filters">List of Nest queries to filter <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html#combining-queries"/></param>
        Task<IList<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters);

        /// <param name="offset">Starting point as a count for filter</param>
        /// <param name="limit">Records should be returned up to a maximum limit.</param>
        ///<inheritdoc cref="FilterAsync(List{Func{QueryContainerDescriptor{T}, QueryContainer}}"/>
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit);
        
        /// <param name="sortFieldName">Sorting field name</param>
        /// <param name="sortByDescending">Sorting direction</param>
        ///<inheritdoc cref="FilterAsync(List{Func{QueryContainerDescriptor{T}, QueryContainer}}"/>
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, string sortFieldName, bool? sortByDescending = null);
        
        ///<inheritdoc cref="FilterAsync(List{Func{QueryContainerDescriptor{T}, QueryContainer}},int?,int?"/>
        /// <param name="sort">Sort query <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/sort-usage.html#sort-usage"/></param>
        Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, SortDescriptor<T> sort, int? offset, int? limit);
        
        //TODO: Remove this, it's not related to Carbon
        ///<inheritdoc cref="FilterAsync(List{Func{QueryContainerDescriptor{T}, QueryContainer}}, int?, int?)"/>
        Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit);
        
        //TODO: Remove this, it's not related to Carbon
        /// <param name="sort">Sort query for sorting. <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/sort-usage.html#sort-usage"/></param>
        ///<inheritdoc cref="FilterAsync(List{Func{QueryContainerDescriptor{T}, QueryContainer}}, int?, int?)"/>
        Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, SortDescriptor<T> sort, int? offset, int? limit);
        
        //TODO: Remove this, it's not related to Carbon
        Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, bool isHierarchical, int? offset, int? limit);
       
        //TODO: Remove this, it's not related to Carbon
        Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, List<string> fieldNames, int? offset, int? limit);
        
        /// <summary>
		/// Returns Count for given query
		/// </summary>
		/// <remarks> <see href="https://www.elastic.co/guide/en/elasticsearch/reference/master/search-count.html"/> </remarks>
		/// <param name="selector">Count request, for detail <see href="https://www.elastic.co/guide/en/elasticsearch/reference/master/search-count.html"/></param>
		/// <returns><see cref="CountResponse"/></returns>
		CountResponse Count(Func<CountDescriptor<T>, ICountRequest> selector = null);
        
        /// <summary>
		/// Returns Count for given query
		/// </summary>
		/// <remarks> <see href="https://www.elastic.co/guide/en/elasticsearch/reference/master/search-count.html"/> </remarks>
		/// <param name="selector">Count request, for detail <see href="https://www.elastic.co/guide/en/elasticsearch/reference/master/search-count.html"/></param>
		/// <returns><see cref="CountResponse"/></returns>
		Task<CountResponse> CountAsync(Func<CountDescriptor<T>, ICountRequest> selector = null);

        #region SearchAsync
        /// <summary>
        /// Returns search response <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html#_search_response"/>
        /// </summary>
        /// <param name="selector">Nest search request <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/reference-search.html"/></param>
        Task<ISearchResponse<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null);
        
        /// <summary>
        /// Sort and search the elastic index using a query and sorting descriptors  
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search. 
        /// <para><see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/sort-usage.html#sort-usage"/></para></param>
        /// <remarks>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access 
        /// further items instead.</remarks>
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor);
        
        /// <summary>
        /// Sort and search the elastic index using a query and orderables 
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <remarks>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access 
        /// further items instead.</remarks>
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables);

        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <remarks>
        /// <para>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access further items instead. </para>
        /// <para>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</para>
        /// </remarks>
        ///<inheritdoc cref="SearchAsync(Func{QueryContainerDescriptor{T}, QueryContainer}, int?, int?, Func{SortDescriptor{T}, IPromise{IList{ISort}}})"/>
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive);

        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <remarks>
        /// <para>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access further items instead.</para>
        /// <para>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</para>
        /// </remarks>
        ///<inheritdoc cref="SearchAsync(Func{QueryContainerDescriptor{T}, QueryContainer}, int?, int?, IList{Orderable})"/>
        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive);
        #endregion

        #region SearchAfterAsync
        /// <summary>
        /// Sort and search the elastic index, using a query, sorting descriptors, and a starting item
        /// </summary>
        /// <param name="query">Function that will provide the query for the search. For detailed info about see: <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/writing-queries.html#combining-queries"/></param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search. For detailed info about see: <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/sort-usage.html#sort-usage"/></param>
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor);
        
        /// <summary>
        /// Sort and search the elastic index, using a query, orderables, and a starting item
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables);

        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <remarks>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</remarks>
        ///<inheritdoc cref="SearchAfterAsync(Func{QueryContainerDescriptor{T}, QueryContainer}, IList{object}, int?, Func{SortDescriptor{T}, IPromise{IList{ISort}}})"/>
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive);

        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <remarks>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</remarks>
        ///<inheritdoc cref="SearchAfterAsync(Func{QueryContainerDescriptor{T}, QueryContainer}, IList{object}, int?, IList{Orderable})"/>
        Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive);
        #endregion

        #region Scrolling
        /// <summary>
        /// Sort and search the elastic index using a query and a sorting descriptor, and then create a scrolling window to access further results ion the future.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="scrollTimeToLive">Amount of time the scroll window instance will be retained after the search</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search</param>
        Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor);
        
        /// <summary>
        /// Sort and search the elastic index using a query and orderables, and then create a scrolling window to access further results ion the future.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="scrollTimeToLive">Amount of time the scroll window instance will be retained after the search</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, IList<Orderable> orderables);
        /// <summary>
        /// Get the next batch of search results from a scrolling window
        /// </summary>
        /// <param name="scrollId">The Id of the scrolling window created by a previous search</param>
        /// <param name="scrollTimeToLive">Amount of time the scroll window instance will be retained after the search</param>
        Task<ISearchResponse<T>> GetNextPageOfScrollAsync(string scrollId, string scrollTimeToLive);
        /// <summary>
        /// Destroy the scrolling window instance
        /// </summary>
        /// <param name="scrollId">The Id of the scrolling window to be destroyed.</param>
        /// <remarks>It is recommended to destroy the scrolling window as soon as it is no longer needed, to free up memory.</remarks>
        Task<ClearScrollResponse> ClearScrollAsync(string scrollId);
        #endregion

        /// <summary>
        /// Creates and returns a new "point in time" instance.
        /// </summary>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained.</param>
        /// <remarks>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</remarks>
        Task<OpenPointInTimeResponse> OpenPointInTimeAsync(string pitTimeToLive);

        /// <summary>
        /// Destroy the point in time instance
        /// </summary>
        /// <param name="scrollId">The Id of the point in time instance to be destroyed.</param>
        /// <remarks>It is recommended to destroy the instances as soon as they are no longer needed, to free up memory.
        /// <para>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</para>
        /// </remarks>
        Task<ClosePointInTimeResponse> ClosePointInTimeAsync(string pitId);
    }
}
