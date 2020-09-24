using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;
using Carbon.Test.Common.DataShares;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;
using Moq;
using System.Collections;
using System.Threading.Tasks;
using Carbon.Domain.Abstractions.UOW;
using Carbon.Test.Common.Fixtures;

namespace Carbon.Domain.EntityFrameworkCore.UnitTests
{
    public class UnitOfWorkFacts : UnitOfWorkTest<TestCarbonContext> { public UnitOfWorkFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { } }
    public abstract class UnitOfWorkTest<T> where T : CarbonContext<T>, IDisposable
    {
        private ITestOutputHelper testOutputHelper;

        public UnitOfWorkTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [EmptyData]
        public async Task SaveChanges_Successfully_Integer()
        {
            var uow = new UnitOfWork<TestCarbonContext>(EFRepositoryFixture.CreateContext());

            var response = uow.SaveChanges();

            Assert.IsType<int>(response);
        }

        [Theory]
        [EmptyData]
        public async Task SaveChangesAsync_Successfully_Integer()
        {
            var uow = new UnitOfWork<TestCarbonContext>(EFRepositoryFixture.CreateContext());

            var response = await uow.SaveChangesAsync();

            Assert.IsType<int>(response);
        }

        [Theory]
        [EmptyData]
        public async Task Dispose_Successfully_Null()
        {
            var uow = new UnitOfWork<TestCarbonContext>(EFRepositoryFixture.CreateContext());

            uow.Dispose();

            Assert.IsType<UnitOfWork<TestCarbonContext>>(uow);
        }
    }
}
