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

        

        //[Fact]
        //public void PagedListOk_Successfully_ReturnStatusCode()
        //{
        //    // Act
        //    var theController = new TestController();

        //    IPagedList<object> thePagedList = new TPagedListClass<object>();

        //    var result = theController.PagedListOkTest<object>(thePagedList);

        //    // Assert
        //    Assert.IsType<OkObjectResult>(result);
        //}

        //[Fact]
        //public void AddParameter_Successfully_ReturnStatusCode()
        //{
            
        //    // Act
        //    var theController = new TestController();

        //    var orderable = new Orderable() { Value = "InsertedDate", IsAscending = true };
        //    var theList = new List<Orderable>();
        //    theList.Add(orderable);

        //    theController.AddParameterTest("X - Paging - Previous - Link", theList, 250, 1);

        //    // Assert
        //    Assert.True(true);
        //}
    }

}
