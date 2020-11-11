using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.WebApplication.UnitTests.DataShares
{
    public class Data : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("name", "invalid");
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("X-CorrelationId", Guid.NewGuid().ToString());


            var actionContext = new ActionContext(
                httpContext,
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            );

            yield return new object[] { actionExecutingContext };
        }
    }
}