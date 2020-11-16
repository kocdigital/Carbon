using Xunit;
using System.Collections.Generic;
using System;
using Carbon.Common;
using Microsoft.AspNetCore.Mvc;
using Carbon.WebApplication.UnitTests.DataShares;
using Carbon.PagedList;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections;
using Moq;

namespace Carbon.WebApplication.UnitTests
{
    public class CarbonControllerTest
    {
        private readonly IEnumerable<string> dataList;

        [Fact]
        public void ResponseResult_Successfully_ReturnStatusCode()
        {
            // Act
            var theController = new TestController();

            var obj = new ApiResponse<object>("identifier", ApiStatusCode.OK);
            var result = theController.ResponseResultTest<ApiResponse<object>>(obj);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void ResponseConflict_Successfully_ReturnStatusCode()
        {
            // Act
            var theController = new TestController();

            var obj = new ApiResponse<object>("identifier", ApiStatusCode.OK);
            var result = theController.ResponseConflictTest<ApiResponse<object>>(obj);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void ResponseNotFound_Successfully_ReturnStatusCode()
        {
            // Act
            var theController = new TestController();

            var obj = new ApiResponse<object>("identifier", ApiStatusCode.OK);
            var result = theController.ResponseNotFoundTest<ApiResponse<object>>(obj);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void ResponseOk_Successfully_ReturnStatusCode()
        {
            // Act
            var theController = new TestController();

            var obj = new ApiResponse<object>("identifier", ApiStatusCode.OK);
            var result = theController.ResponseOkTest<ApiResponse<object>>(obj);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ResponseOkWithoutT_Successfully_ReturnStatusCode()
        {
            // Act
            var theController = new TestController();

            var result = theController.ResponseOkWithoutTTest();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void UpdatedOk_Successfully_ReturnStatusCode()
        {
            // Act
            var theController = new TestController();

            var obj = new ApiResponse<object>("identifier", ApiStatusCode.OK);

            var anObject = new object();

            var result = theController.UpdatedOkTest<ApiResponse<object>>("actionName", anObject, obj);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public void DeletedOk_Successfully_ReturnStatusCode()
        {
            // Act
            var theController = new TestController();

            var result = theController.DeletedOkTest();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void CreatedOk_Successfully_ReturnStatusCode()
        {
            // Act
            var theController = new TestController();

            var obj = new ApiResponse<object>("identifier", ApiStatusCode.OK);

            var anObject = new object();

            var result = theController.CreatedOkTest<ApiResponse<object>>("actionName", anObject, obj);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public void PagedListOk_Successfully_ReturnStatusCode()
        {
            // Act
            var headerDictionary = new HeaderDictionary();
            var response = new Mock<HttpResponse>();
            response.SetupGet(r => r.Headers).Returns(headerDictionary);

            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Response == response.Object
            );
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var theController = new TestController() {
                ControllerContext = controllerContext
            };

            IPagedList<object> thePagedList = new TPagedListClass<object>();

            var result = theController.PagedListOkTest<object>(thePagedList);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void AddParameter_Successfully_ReturnStatusCode()
        {

            // Act
            var headerDictionary = new HeaderDictionary();
            var response = new Mock<HttpResponse>();
            response.SetupGet(r => r.Headers).Returns(headerDictionary);

            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));
            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Request == request.Object &&
                _.Response == response.Object
            );
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var theController = new TestController()
            {
                ControllerContext = controllerContext
            };

            var orderable = new Orderable() { Value = "InsertedDate", IsAscending = true };
            var theList = new List<Orderable>();
            theList.Add(orderable);

            theController.AddParameterTest("X - Paging - Previous - Link", theList, 250, 1);

            // Assert
            Assert.True(true);
        }

        [Fact]
        public void PagedOk_Successfully_ReturnStatusCode()
        {
            // Act
            var headerDictionary = new HeaderDictionary();
            var response = new Mock<HttpResponse>();
            response.SetupGet(r => r.Headers).Returns(headerDictionary);

            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));

            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Request == request.Object &&
                _.Response == response.Object
            );

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var theController = new TestController()
            {
                ControllerContext = controllerContext
            };

            IPagedList<object> thePagedList = new TPagedListClass<object>();

            var result = theController.PagedOkTest<object>(thePagedList, new OrdinatedPageDto(), 250);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        public class PagedListClass : IPagedList
        {
            public int PageCount => 1;

            public int TotalItemCount => 1;

            public int PageNumber => 1;

            public int PageSize => 1;

            public bool HasPreviousPage => false;

            public bool HasNextPage => false;

            public bool IsFirstPage => true;

            public bool IsLastPage => true;

            public int FirstItemOnPage => 1;

            public int LastItemOnPage => 1;
        }

        public class TPagedListClass<T> : IPagedList<T>
        {
            public T this[int index] => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public int PageCount => 1;

            public int TotalItemCount => 1;

            public int PageNumber => 1;

            public int PageSize => 1;

            public bool HasPreviousPage => false;

            public bool HasNextPage => false;

            public bool IsFirstPage => true;

            public bool IsLastPage => true;

            public int FirstItemOnPage => 1;

            public int LastItemOnPage => 1;

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public IPagedList GetMetaData()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }

}
