using Carbon.Common;
using Carbon.ElasticSearch.Abstractions;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.ElasticSearch
{
    /// <inheritdoc cref="IElasticReadOnlyRepository{T}"/>
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
        public async Task<T> FindOneAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(0).Size(1).Query(query), cancellationToken);

            return response.Documents.FirstOrDefault();
        }
        public async Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(0).Size(1000).Query(query), cancellationToken);

            return response.Documents.ToList();
        }

        public async Task<IEnumerable<T>> FindAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int size, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x => x.Index(Index).From(0).Size(size > 0 ? size : 1000).Query(query), cancellationToken);

            return response.Documents.ToList();
        }

        public async Task<ISearchResponse<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null, CancellationToken cancellationToken = default)
        {
            return await _client.SearchAsync<T>(selector, cancellationToken);
        }
        public async Task<IList<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index).Size(1000).Query(q => q.Bool(bq => bq.Filter(filters.ToArray())));
                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response.Documents.ToList();
        }
        public async Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index).From(offset).Size(limit).Query(q => q.Bool(bq => bq.Filter(filters.ToArray())));
                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public async Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, string sortFieldName, bool? sortByDescending = null, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .From(offset)
                    .Size(limit)
                    .Query(q => q.Bool(bq => bq.Filter(filters.ToArray())))
                    .Sort(s => sortByDescending ?? false ? s.Descending($"{sortFieldName}.keyword") : s.Ascending($"{sortFieldName}.keyword"));
                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public async Task<ISearchResponse<T>> FilterAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, SortDescriptor<T> sort, int? offset, int? limit, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .From(offset)
                    .Size(limit)
                    .Query(q => q.Bool(bq => bq.Filter(filters.ToArray())))
                    .Sort(s => sort);
                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public async Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, int? offset, int? limit, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .From(offset)
                    .Size(limit)
                    .Query(q => q.Bool(bq => bq.Filter(filters.ToArray())));
                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public async Task<ISearchResponse<T>> FilterDiscoveryAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, SortDescriptor<T> sort, int? offset, int? limit, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .From(offset)
                    .Size(limit)
                    .Query(q => q.Bool(bq => bq.Filter(filters.ToArray())))
                    .Sort(s => sort);
                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public async Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, bool isHierarchical, int? offset, int? limit, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var field = isHierarchical ? "hierarchicalUrl" : "id";
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .From(offset)
                    .Size(limit)
                    .Source(sr => sr.Includes(fi => fi.Field(field)))
                    .Query(q => q.Bool(bq => bq.Filter(filters.ToArray())));

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }

                return x;
            }, cancellationToken);
            return response;
        }

        public async Task<ISearchResponse<T>> DiscoverUrlAsync(List<Func<QueryContainerDescriptor<T>, QueryContainer>> filters, List<string> fieldNames, int? offset, int? limit, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var fieldList = fieldNames.Select(f => (Field)f).ToList();
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .From(offset)
                    .Size(limit)
                    .Source(sr => sr.Includes(fi => fi.Fields(fieldList)))
                    .Query(q => q.Bool(bq => bq.Filter(filters.ToArray())));

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public CountResponse Count(Func<CountDescriptor<T>, ICountRequest> selector = null)
        {
            return _client.Count<T>(selector);
        }
        public async Task<CountResponse> CountAsync(Func<CountDescriptor<T>, ICountRequest> selector = null, CancellationToken cancellationToken = default)
        {
            return await _client.CountAsync<T>(selector, cancellationToken);
        }

        #region SearchAsync

        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .Query(query)
                    .From(from)
                    .Size(size)
                    .Sort(sortDescriptor);
                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .Query(query)
                    .From(from)
                    .Size(size)
                    .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables));

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }

                return x;
            }, cancellationToken);
            return response;
        }

        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var activePitId = pitId;
            if (string.IsNullOrEmpty(activePitId))
            {
                var openPitResponse = await OpenPointInTimeAsync(pitTimeToLive, cancellationToken);
                activePitId = openPitResponse.Id;
            }
            var response = await _client.SearchAsync<T>(x =>
            {
                x.PointInTime(activePitId, pit => pit.KeepAlive(pitTimeToLive))
                    .Query(query)
                    .From(from)
                    .Size(size)
                    .Sort(sortDescriptor);

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, int? from, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var activePitId = pitId;
            if (string.IsNullOrEmpty(activePitId))
            {
                var openPitResponse = await OpenPointInTimeAsync(pitTimeToLive, cancellationToken);
                activePitId = openPitResponse.Id;
            }
            var response = await _client.SearchAsync<T>(x =>
            {
                x.PointInTime(activePitId, pit => pit.KeepAlive(pitTimeToLive))
                    .Query(query)
                    .From(from)
                    .Size(size)
                    .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables));
                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);
            return response;
        }

        #endregion

        #region SearchAfterAsync

        public async Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .Query(query)
                    .SearchAfter(searchAfter)
                    .Size(size)
                    .Sort(sortDescriptor);

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);
            return response;
        }

        public async Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .Query(query)
                    .SearchAfter(searchAfter)
                    .Size(size)
                    .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables));

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);
            return response;
        }

        public async Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, string pitId, string pitTimeToLive, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var activePitId = pitId;
            if (string.IsNullOrEmpty(activePitId))
            {
                var openPitResponse = await OpenPointInTimeAsync(pitTimeToLive, cancellationToken);
                activePitId = openPitResponse.Id;
            }

            var response = await _client.SearchAsync<T>(x =>
            {
                x.PointInTime(activePitId, pit => pit.KeepAlive(pitTimeToLive))
                    .Query(query)
                    .SearchAfter(searchAfter)
                    .Size(size)
                    .Sort(sortDescriptor);

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public async Task<ISearchResponse<T>> SearchAfterAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, IList<object> searchAfter, int? size, IList<Orderable> orderables, string pitId, string pitTimeToLive, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var activePitId = pitId;
            if (string.IsNullOrEmpty(activePitId))
            {
                var openPitResponse = await OpenPointInTimeAsync(pitTimeToLive, cancellationToken);
                activePitId = openPitResponse.Id;
            }

            var response = await _client.SearchAsync<T>(x =>
            {
                x.PointInTime(activePitId, pit => pit.KeepAlive(pitTimeToLive))
                    .Query(query)
                    .SearchAfter(searchAfter)
                    .Size(size)
                    .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables));

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);
            return response;
        }

        #endregion

        #region Scrolling

        public async Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortDescriptor, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .Query(query)
                    .Scroll(scrollTimeToLive)
                    .Size(size)
                    .Sort(sortDescriptor);

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);
            return response;
        }

        public async Task<ISearchResponse<T>> CreateScrollingSearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> query, string scrollTimeToLive, int size, IList<Orderable> orderables, bool trackTotalHits = false, CancellationToken cancellationToken = default)
        {
            var response = await _client.SearchAsync<T>(x =>
            {
                x.Index(Index)
                    .Query(query)
                    .Scroll(scrollTimeToLive)
                    .Size(size)
                    .Sort(sd => GenerateSortDescriptorFromOrderables(sd, orderables));

                if (trackTotalHits)
                {
                    x.TrackTotalHits();
                }
                return x;
            }, cancellationToken);

            return response;
        }

        public async Task<ISearchResponse<T>> GetNextPageOfScrollAsync(string scrollId, string scrollTimeToLive, CancellationToken cancellationToken = default)
        {
            var response = await _client.ScrollAsync<T>(new ScrollRequest(scrollId, scrollTimeToLive), cancellationToken);

            return response;
        }

        public async Task<ClearScrollResponse> ClearScrollAsync(string scrollId, CancellationToken cancellationToken = default)
        {
            return await _client.ClearScrollAsync(new ClearScrollRequest(scrollId), cancellationToken);
        }

        #endregion

        
        public async Task<OpenPointInTimeResponse> OpenPointInTimeAsync(string pitTimeToLive, CancellationToken cancellationToken = default)
        {
            return await _client.OpenPointInTimeAsync(Index, pit => pit.KeepAlive(pitTimeToLive), cancellationToken);
        }

        
        public async Task<ClosePointInTimeResponse> ClosePointInTimeAsync(string pitId, CancellationToken cancellationToken = default)
        {
            return await _client.ClosePointInTimeAsync(x => x.Id(pitId), cancellationToken);
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

