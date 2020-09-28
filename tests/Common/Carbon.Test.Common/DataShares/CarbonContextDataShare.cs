using Carbon.Common;
using Carbon.Domain.Abstractions.Entities;
using Carbon.Domain.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Carbon.Test.Common.DataShares
{
    public class EmptyData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] {  };
        }
    }

    public class SaveChanges : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { true };
        }
    }

    public class CarbonContextTestClass :IEntity, IMustHaveTenant
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; }
    }

    public class TestCarbonContext : CarbonContext<TestCarbonContext>
    {
        public TestCarbonContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CarbonContextTestClass> CarbonContextTestClass { get; set; }
    }
    
}
