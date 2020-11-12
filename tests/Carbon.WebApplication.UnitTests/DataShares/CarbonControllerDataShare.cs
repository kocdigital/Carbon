using Carbon.Common;
using Carbon.PagedList;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace Carbon.WebApplication.UnitTests.DataShares
{
    public class TestController : CarbonController
    {
        public TestController() : base()
        {
        }

        public ObjectResult ResponseResultTest<T>(T value) where T : IApiResponse
        {
            return base.ResponseResult<T>(value);
        }

        public ObjectResult ResponseConflictTest<T>(T value)
        {
            return base.ResponseConflict<T>(value);
        }

        public ObjectResult ResponseNotFoundTest<T>(T value)
        {
            return base.ResponseNotFound<T>(value);
        }

        public OkObjectResult ResponseOkTest<T>(T value)
        {
            return base.ResponseOk<T>(value);
        }

        public OkObjectResult ResponseOkWithoutTTest()
        {
            return base.ResponseOk();
        }

        public CreatedAtActionResult UpdatedOkTest<T>(string actionName, object routeValues, T value)
        {
            return base.UpdatedOk<T>(actionName, routeValues, value);
        }

        public OkObjectResult DeletedOkTest()
        {
            return base.DeletedOk();
        }

        public CreatedAtActionResult CreatedOkTest<T>(string actionName, object routeValues, T value)
        {
            return base.CreatedOk<T>(actionName, routeValues, value);
        }

        public OkObjectResult PagedListOkTest<T>(IPagedList<T> entity)
        {
            return base.PagedListOk<T>(entity);
        }

        public void AddParameterTest(string key, IList<Orderable> ordination, int pageSize, int pageIndex)
        {
            base.AddParameter(key, ordination, pageSize, pageIndex);
        }
    }

    //public class PagedListClass : IPagedList
    //{
    //    public int PageCount => 1;

    //    public int TotalItemCount => 1;

    //    public int PageNumber => 1;

    //    public int PageSize => 1;

    //    public bool HasPreviousPage => false;

    //    public bool HasNextPage => false;

    //    public bool IsFirstPage => true;

    //    public bool IsLastPage => true;

    //    public int FirstItemOnPage => 1;

    //    public int LastItemOnPage => 1;
    //}

    //public class TPagedListClass<T> : IPagedList<T>
    //{
    //    public T this[int index] => throw new NotImplementedException();

    //    public int Count => throw new NotImplementedException();

    //    public int PageCount => 1;

    //    public int TotalItemCount => 1;

    //    public int PageNumber => 1;

    //    public int PageSize => 1;

    //    public bool HasPreviousPage => false;

    //    public bool HasNextPage => false;

    //    public bool IsFirstPage => true;

    //    public bool IsLastPage => true;

    //    public int FirstItemOnPage => 1;

    //    public int LastItemOnPage => 1;

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IPagedList GetMetaData()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
