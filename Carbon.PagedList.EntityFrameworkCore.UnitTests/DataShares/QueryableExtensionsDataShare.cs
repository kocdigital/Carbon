using Carbon.Test.Common.Fixtures;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.PagedList.EntityFrameworkCore.UnitTests.DataShares
{
    public class ToPagedListQueryableExtensions : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var context = EFRepositoryFixture.CreateContext();
            EFRepositoryFixture.CreateData(context, Guid.NewGuid(), Guid.NewGuid());

            //data, pageNumber, pageSize
            yield return new object[] { context.CarbonContextTestClass, 1, 1 };
            yield return new object[] { null, 10, 2 };
        }
    }

    public class InValidToPagedListQueryableExtensions : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var context = EFRepositoryFixture.CreateContext();
            EFRepositoryFixture.CreateData(context, Guid.NewGuid(), Guid.NewGuid());

            //data, pageNumber, pageSize
            yield return new object[] { context.CarbonContextTestClass, -1, -1 };
            yield return new object[] { null, -10, -2 };
        }
    }
}