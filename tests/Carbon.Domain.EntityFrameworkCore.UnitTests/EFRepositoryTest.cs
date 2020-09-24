using Carbon.Domain.Abstractions.Entities;
using Carbon.Test.Common.DataShares;
using Carbon.Test.Common.Fixtures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Carbon.Domain.EntityFrameworkCore.UnitTests
{

    public class EFRepositoryTest : EFRepository<CarbonContextTestClass, TestCarbonContext>
    {

        public EFRepositoryTest() : base(EFRepositoryFixture.CreateContext())
        {

        }
     
        [Theory]
        [FoundEntityGetByIdAsyncEFRepository]
        public async Task GetByIdAsync_Successfully_ReturnEntity(Guid Id)
        {
            // Arrange
            EFRepositoryFixture.CreateData(base.context, Id);
              // Act
              var response = await base.GetByIdAsync(Id);

            // Assert
            Assert.Equal("Name 1", response.Name);
        }
        [Theory]
        [NotFoundEntityGetByIdAsyncEFRepository]
        public async Task GetByIdAsync_Successfully_ReturnNull(Guid Id)
        {
            // Arrange
            EFRepositoryFixture.CreateData(base.context, Guid.NewGuid());
            // Act
            var response = await base.GetByIdAsync(Id);

            // Assert
            Assert.Null(response);
        }
    }
}
