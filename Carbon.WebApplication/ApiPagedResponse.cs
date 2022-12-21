using Carbon.Common;
using Carbon.PagedList;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.WebApplication
{
    public class ApiPagedResponse<T> : ApiResponse<T>, IPagedList where T : IPagedList
    {
        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public bool HasPreviousPage { get; private set; }
        public bool HasNextPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }
        public int FirstItemOnPage { get; private set; }
        public int LastItemOnPage { get; private set; }

        public ApiPagedResponse(string identifier, ApiStatusCode statusCode, T data) : base(identifier, statusCode)
        {
            Data = data;
            SetPaging(data);
        }
        private void SetPaging(IPagedList pagedList)
        {
            PageCount = pagedList.PageCount;
            TotalItemCount = pagedList.TotalItemCount;
            PageNumber = pagedList.PageNumber;
            PageSize = pagedList.PageSize;
            HasPreviousPage = pagedList.HasPreviousPage;
            HasNextPage = pagedList.HasNextPage;
            IsFirstPage = pagedList.IsFirstPage;
            IsLastPage = pagedList.IsLastPage;
            FirstItemOnPage = pagedList.FirstItemOnPage;
            LastItemOnPage = pagedList.LastItemOnPage;
        }
    }
}
