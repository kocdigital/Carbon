using Carbon.Test.Common.DataShares;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Moq;
using System.Collections;
using System.Threading.Tasks;

namespace Carbon.Domain.EntityFrameworkCore.UnitTests
{
    public class DbContextFacts : CarbonContextTest<DbContext> { public DbContextFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }

    public abstract class CarbonContextTest<TContext> : DbContext where TContext : DbContext
    {
        private readonly ITestOutputHelper _testOutputHelper;

        protected CarbonContextTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        #region SaveChanges
        [Theory]
        [SaveChanges]
        public async Task SaveChanges_Successfully_Integer(bool acceptAllChangesOnSuccess)
        {
            var options = new DbContextOptionsBuilder<TestContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            using (var context = new TestContext(options))
            {
                context.TestClass.Add(new TestClass { Id = Guid.NewGuid(), Name = "Name 1" });
                context.TestClass.Add(new TestClass { Id = Guid.NewGuid(), Name = "Name 2" });
                var response = context.SaveChanges(acceptAllChangesOnSuccess);
                Assert.IsType<int>(response);
            }


            _testOutputHelper.WriteLine("Test passed!");
        }
        #endregion

        #region SaveChangesAsync
        [Theory]
        [SaveChanges]
        public async Task SaveChangesAsync_Successfully_Integer(bool acceptAllChangesOnSuccess)
        {
            var options = new DbContextOptionsBuilder<TestContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            using (var context = new TestContext(options))
            {
                context.TestClass.Add(new TestClass { Id = Guid.NewGuid(), Name = "Name 1" });
                context.TestClass.Add(new TestClass { Id = Guid.NewGuid(), Name = "Name 2" });
                var response = await context.SaveChangesAsync(acceptAllChangesOnSuccess);
                Assert.IsType<int>(response);
            }


            _testOutputHelper.WriteLine("Test passed!");
        }
        #endregion
    }
}
