using Nest;
using Carbon.ElasticSearch.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Carbon.Common;

namespace Carbon.ElasticSearch
{

    public abstract class BaseElasticReadOnlyRepository<T> : IElasticReadOnlyRepository<T> where T : class
    {
        private protected readonly ElasticClient _client;
        private protected readonly IElasticSettings _elasticSettings;
        public abstract string Index { get; }

        protected BaseElasticReadOnlyRepository(IElasticSettings elasticSettings)
        {
            _client = new ElasticClient(elasticSettings.ConnectionSettings);
            _elasticSettings = elasticSettings;
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
        
        public async Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters,SortDescriptor<T> sort, int? offset, int? limit)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .From(offset)
                .Size(limit)
                .Query(q => q.Bool(bq => bq.Filter(filters.ToArray())))
                .Sort(s=> sort));
            return response;
        }

        public async Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(offset).Size(limit).Query(q => q.Bool(bq => bq.Filter(filters.ToArray()))));
            return response;
        }
        public async Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, SortDescriptor<T> sort, int? offset, int? limit)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(offset).Size(limit).Query(q => q.Bool(bq => bq.Filter(filters.ToArray()))).Sort(s=> sort));
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

        #region SearchAsync

        /// <summary>
        /// Sort and search the elastic index using a query and sorting descriptors  
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search</param>
        /// <remarks>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access 
        /// further items instead.</remarks>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .Query(query)
                .From(from)
                .Size(size)
                .Sort(sortDescriptor));

            return response;
        }

        /// <summary>
        /// Sort and search the elastic index using a query and orderables 
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <remarks>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access 
        /// further items instead.</remarks>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .Query(query)
                .From(from)
                .Size(size)
                .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables)));

            return response;
        }

        /// <summary>
        /// Sort and search the elastic index in a point of time, using a query and sorting descriptors 
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search</param>
        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <remarks>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access 
        /// further items instead. 
        /// <para>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</para>
        /// </remarks>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive)
        {
            var activePitId = pitId;
            if (string.IsNullOrEmpty(activePitId))
            {
                var openPitResponse = await OpenPointInTimeAsync(pitTimeToLive);
                activePitId = openPitResponse.Id;
            }

            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .PointInTime(activePitId, pit => pit.KeepAlive(pitTimeToLive))
                .Query(query)
                .From(from)
                .Size(size)
                .Sort(sortDescriptor));

            return response;
        }

        /// <summary>
        /// Sort and search the elastic index in a point of time, using a query and orderables 
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="from">Number of items to skip when returning results</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <remarks>This method of searching is not supported for accessing beyond first 10000 results of the given query. Use SearchAfterAsync method to access 
        /// further items instead.
        /// <para>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</para>
        /// </remarks>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive)
        {
            var activePitId = pitId;
            if (string.IsNullOrEmpty(activePitId))
            {
                var openPitResponse = await OpenPointInTimeAsync(pitTimeToLive);
                activePitId = openPitResponse.Id;
            }

            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .PointInTime(activePitId, pit => pit.KeepAlive(pitTimeToLive))
                .Query(query)
                .From(from)
                .Size(size)
                .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables)));

            return response;
        }
        #endregion

        #region SearchAfterAsync

        /// <summary>
        /// Sort and search the elastic index, using a query, sorting descriptors, and a starting item
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search</param>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .Query(query)
                .SearchAfter(searchAfter)
                .Size(size)
                .Sort(sortDescriptor));

            return response;
        }

        /// <summary>
        /// Sort and search the elastic index, using a query, orderables, and a starting item
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .Query(query)
                .SearchAfter(searchAfter)
                .Size(size)
                .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables)));

            return response;
        }

        /// <summary>
        /// Sort and search the elastic index in a point of time, using a query, sorting descriptors and a starting item
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search</param>
        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <remarks>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</remarks>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive)
        {
            var activePitId = pitId;
            if (string.IsNullOrEmpty(activePitId))
            {
                var openPitResponse = await OpenPointInTimeAsync(pitTimeToLive);
                activePitId = openPitResponse.Id;
            }

            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .PointInTime(activePitId, pit => pit.KeepAlive(pitTimeToLive))
                .Query(query)
                .SearchAfter(searchAfter)
                .Size(size)
                .Sort(sortDescriptor));

            return response;

        }

        /// <summary>
        /// Sort and search the elastic index in a point of time, using a query, orderables and a starting item
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="searchAfter">Sorting key of the item after which the search will begin</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <param name="pitId">The Id for the "point in time" that the search will take place in.
        /// If a null value or empty string is provided, a new "point in time" instance will be created and used.</param>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained after the search.</param>
        /// <remarks>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</remarks>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive)
        {
            var activePitId = pitId;
            if (string.IsNullOrEmpty(activePitId))
            {
                var openPitResponse = await OpenPointInTimeAsync(pitTimeToLive);
                activePitId = openPitResponse.Id;
            }

            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                .PointInTime(activePitId, pit => pit.KeepAlive(pitTimeToLive))
                .Query(query)
                .SearchAfter(searchAfter)
                .Size(size)
                .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables)));

            return response;

        }
        #endregion

        #region Scrolling

        /// <summary>
        /// Sort and search the elastic index using a query and a sorting descriptor, and then create a scrolling window to access further results ion the future.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="scrollTimeToLive">Amount of time the scroll window instance will be retained after the search</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="sortDescriptor">Function that will provide the sorting order for the search</param>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                  .Query(query)
                  .Scroll(scrollTimeToLive)
                  .Size(size)
                  .Sort(sortDescriptor));

            return response;
        }

        /// <summary>
        /// Sort and search the elastic index using a query and orderables, and then create a scrolling window to access further results ion the future.
        /// </summary>
        /// <param name="query">Function that will provide the query for the search</param>
        /// <param name="scrollTimeToLive">Amount of time the scroll window instance will be retained after the search</param>
        /// <param name="size">Number of items to be returned</param>
        /// <param name="orderables">List of orderables that will provide the sorting order for the search</param>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, IList<Orderable> orderables)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index)
                  .Query(query)
                  .Scroll(scrollTimeToLive)
                  .Size(size)
                  .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables)));

            return response;
        }

        /// <summary>
        /// Get the next batch of search results from a scrolling window
        /// </summary>
        /// <param name="scrollId">The Id of the scrolling window created by a previous search</param>
        /// <param name="scrollTimeToLive">Amount of time the scroll window instance will be retained after the search</param>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> GetNextPageOfScrollAsync(string scrollId, string scrollTimeToLive)
        {
            var response = await _client.ScrollAsync<T>(new ScrollRequest(scrollId, scrollTimeToLive));

            return response;
        }

        /// <summary>
        /// Destroy the scrolling window instance
        /// </summary>
        /// <param name="scrollId">The Id of the scrolling window to be destroyed.</param>
        /// <remarks>It is recommended to destroy the scrolling window as soon as it is no longer needed, to free up memory.</remarks>
        /// <returns></returns>
        public async Task<ClearScrollResponse> ClearScrollAsync(string scrollId)
        {
            return await _client.ClearScrollAsync(new ClearScrollRequest(scrollId));
        }

        #endregion

        /// <summary>
        /// Creates and returns a new "point in time" instance.
        /// </summary>
        /// <param name="pitTimeToLive">Amount of time the "point in time" instance will be retained.</param>
        /// <remarks>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</remarks>
        /// <returns></returns>
        public async Task<OpenPointInTimeResponse> OpenPointInTimeAsync(string pitTimeToLive)
        {
            return await _client.OpenPointInTimeAsync(Index, pit => pit.KeepAlive(pitTimeToLive));
        }

        /// <summary>
        /// Destroy the point in time instance
        /// </summary>
        /// <param name="scrollId">The Id of the point in time instance to be destroyed.</param>
        /// <remarks>It is recommended to destroy the instances as soon as they are no longer needed, to free up memory.
        /// <para>Point in time feature is only supported for ElasticSearch versions 7.10 and above.</para>
        /// </remarks>
        /// <returns></returns>
        public async Task<ClosePointInTimeResponse> ClosePointInTimeAsync(string pitId)
        {
            return await _client.ClosePointInTimeAsync(x => x.Id(pitId));
        }

        private IPromise<IList<ISort>> GenerateSortDescriptorFromOrderables(SortDescriptor<T> baseSortDescriptor, IList<Orderable> orderables)
        {
            var ret = baseSortDescriptor;

            if (orderables?.Count > 0)
            {
                for (int i = 0; i < orderables.Count; i++)
                {
                    ret = ret.Field(orderables[i].Value, orderables[i].IsAscending ? SortOrder.Ascending : SortOrder.Descending);
                }
            }

            return ret;
        }
    }
}

