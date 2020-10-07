using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Test.Common.DataShares
{
    public class FoundEntityGetByIdAsyncEFRepository : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.Parse("06956e56-1f8b-4fd9-b4b4-f083c23bb98f")};
        }
    }

    public class FoundEntityWithTenantGetByIdAsyncEFRepository : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.Parse("26956e56-1f8b-4fd9-b4b4-f083c23bb98f"), Guid.Parse("36956e56-1f8b-4fd9-b4b4-f083c23bb98f") };
        }
    }

    public class NotFoundEntityGetByIdAsyncEFRepository : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { Guid.NewGuid(), Guid.NewGuid() };
        }
    }

    public class EntityEFRepository : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { new CarbonContextTestClass() { Id = Guid.NewGuid(), TenantId = Guid.NewGuid(), Name = "Test Name"} };
        }
    }
    
    public class CreateRangeEntityEFRepository : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var entity1 = new CarbonContextTestClass() { Id = Guid.NewGuid(), TenantId = Guid.NewGuid(), Name = "Test Name 1" };
            var entity2 = new CarbonContextTestClass() { Id = Guid.NewGuid(), TenantId = Guid.NewGuid(), Name = "Test Name 2" };
            var listEntity = new List<CarbonContextTestClass>();
            listEntity.Add(entity1);
            listEntity.Add(entity2);
            yield return new object[] { listEntity };
        }
    }
}
