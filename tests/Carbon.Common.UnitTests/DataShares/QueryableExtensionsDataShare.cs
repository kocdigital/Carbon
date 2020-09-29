using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Common.UnitTests.DataShares
{
    public static class Data
    {
        public static List<SampleTestClass> ValueList = new List<SampleTestClass>() {
            new SampleTestClass(){Name = "test 1" },
            new SampleTestClass(){Name = "test 2" },
            new SampleTestClass(){Name = "test 3" },
            new SampleTestClass(){Name = "test 4" },
            new SampleTestClass(){Name = "test 5" },
            new SampleTestClass(){Name = "test 6" }};

        public static IEnumerable<SampleTestClass> Query =
          ValueList.AsQueryable();
    }

    public class CalculatePageCountQueryableExtensions : DataAttribute
    {


        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            //data, size
            yield return new object[] { Data.Query, 2 };
            yield return new object[] { Data.Query, null };
            yield return new object[] { Data.Query, -1 };
        }
    }
    public class SkipTakeQueryableExtensions : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            //data, index, size
            yield return new object[] { Data.Query, 2, 1 };
            yield return new object[] { Data.Query, 2, 2 };
        }
    }
    public class InvalidSkipTakeQueryableExtensions : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            //data, index, size
            yield return new object[] { Data.Query, int.MaxValue, 1 };
            yield return new object[] { Data.Query, int.MinValue, 0 };
        }
    }
    public class OrderByQueryableExtensions : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var ordinations = new List<Orderable>();
            ordinations.Add(new Orderable() { IsAscending = true, Value = "Name" });
            ordinations.Add(new Orderable() { IsAscending = false, Value = "Name" });
            ordinations.Add(new Orderable() { IsAscending = false, Value = "" });
            yield return new object[] { Data.Query, ordinations };
            yield return new object[] { Data.Query, null };
        }
    }
    public class EmptyDataOrderByQueryableExtensions : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var ordinations = new List<Orderable>();
            yield return new object[] { Data.Query, ordinations };
        }
    }
}
