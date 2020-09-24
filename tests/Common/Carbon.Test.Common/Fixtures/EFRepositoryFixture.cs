using Carbon.Test.Common.DataShares;
using Microsoft.EntityFrameworkCore;
using System;

namespace Carbon.Test.Common.Fixtures
{
    public class EFRepositoryFixture
    {
        public static TestCarbonContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<TestCarbonContext>()
         .UseInMemoryDatabase(databaseName: "TestDatabase")
         .Options;
            var context = new TestCarbonContext(options);

            return context;
        }

        public static void CreateData(TestCarbonContext context, Guid id)
        {
            context.CarbonContextTestClass.Add(new CarbonContextTestClass { Id = id, Name = "Name 1" });
            context.CarbonContextTestClass.Add(new CarbonContextTestClass { Id = Guid.NewGuid(), Name = "Name 2" });
            context.SaveChanges();
        }
    }
}
