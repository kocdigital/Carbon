using Carbon.Domain.Abstractions.Entities;
using Carbon.Test.Common.DataShares;
using Carbon.Test.Common.Fixtures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Carbon.Domain.EntityFrameworkCore.UnitTests
{

    public class EFTenantRepositoryTest : EFTenantRepository<CarbonContextTestClass, TestCarbonContext>
    {

        public EFTenantRepositoryTest() : base(EFRepositoryFixture.CreateContext())
        {

        }
     
        [Theory]
        [FoundEntityWithTenantGetByIdAsyncEFRepository]
        public async Task GetByIdAsync_Successfully_ReturnEntity(Guid Id, Guid tenantId)
        {
            // Arrange
            EFRepositoryFixture.CreateData(base.context, Id, tenantId);
              // Act
              var response = await base.GetByIdAsync(Id, tenantId);

            // Assert
            Assert.Equal(Id, response.Id);
        }
        [Theory]
        [NotFoundEntityGetByIdAsyncEFRepository]
        public async Task GetByIdAsync_Successfully_ReturnNull(Guid Id, Guid tenantId)
        {
            // Arrange
            EFRepositoryFixture.CreateData(base.context, Guid.NewGuid(), Guid.NewGuid());
            // Act
            var response = await base.GetByIdAsync(Id, tenantId);

            // Assert
            Assert.Null(response);
        }

        [Theory]
        [EntityEFRepository]
        public async Task CreateAsync_Successfully_ReturnEntity(CarbonContextTestClass carbonContextTestClass)
        {
            // Arrange

            // Act
            var response = await base.CreateAsync(carbonContextTestClass);

            // Assert
            Assert.Equal(carbonContextTestClass.Name, response.Name);
        }

        [Theory]
        [EntityEFRepository]
        public async Task UpdateAsync_Successfully_ReturnEntity(CarbonContextTestClass carbonContextTestClass)
        {
            // Arrange
            var createResponse = await base.CreateAsync(carbonContextTestClass);
            createResponse.Name = "Updated";
            // Act
            var response = await base.UpdateAsync(createResponse);

            // Assert
            Assert.Equal(createResponse.Name, response.Name);
        }

        [Theory]
        [EntityEFRepository]
        public async Task DeleteAsync_Successfully_ReturnEntity(CarbonContextTestClass carbonContextTestClass)
        {
            // Arrange
            var createResponse = await base.CreateAsync(carbonContextTestClass);
            // Act
            var response = await base.DeleteAsync(createResponse.Id, createResponse.TenantId);

            // Assert
            Assert.Equal(carbonContextTestClass.Id, response.Id);
        }

        [Theory]
        [EntityEFRepository]
        public async Task GetAllAsync_Successfully_ReturnEntity(CarbonContextTestClass carbonContextTestClass)
        {
            // Arrange
            EFRepositoryFixture.CreateData(base.context, carbonContextTestClass.Id, carbonContextTestClass.TenantId);
            // Act
            var response = await base.GetAllAsync(carbonContextTestClass.TenantId);

            // Assert
            Assert.IsType<List<CarbonContextTestClass>>(response);
        }

        [Theory]
        [CreateRangeEntityEFRepository]
        public async Task CreateRangeAsync_Successfully_ReturnEntity(List<CarbonContextTestClass> carbonContextTestClassList)
        {
            // Arrange

            // Act
            var response = await base.CreateRangeAsync(carbonContextTestClassList);

            // Assert
            Assert.IsType<List<CarbonContextTestClass>>(response);
        }

        [Theory]
        [CreateRangeEntityEFRepository]
        public async Task UpdateRangeAsync_Successfully_ReturnEntity(List<CarbonContextTestClass> carbonContextTestClassList)
        {
            // Arrange
            var createResponse = await base.CreateRangeAsync(carbonContextTestClassList);
            foreach (var item in createResponse)
            {
                var i = 0;
                item.Name += i++.ToString();
            }
            // Act
            var response = await base.UpdateRangeAsync(createResponse);

            // Assert
            for(var a = 0; a < response.Count; a++)
            {
                Assert.Equal(createResponse[a].Name, response[a].Name);
            }
            
        }

        [Theory]
        [CreateRangeEntityEFRepository]
        public async Task DeleteRangeAsync_Successfully_ReturnEntity(List<CarbonContextTestClass> carbonContextTestClassList)
        {
            // Arrange
            var createResponse = await base.CreateRangeAsync(carbonContextTestClassList);

            // Act
            var response = await base.DeleteRangeAsync(createResponse);

            // Assert
            Assert.IsType<List<CarbonContextTestClass>>(response);
        }

        [Theory]
        [CreateRangeEntityEFRepository]
        public async Task GetAsync_Successfully_ReturnEntity(List<CarbonContextTestClass> carbonContextTestClassList)
        {
            // Arrange
            var createResponse = await base.CreateRangeAsync(carbonContextTestClassList);

            // Act
            var response = await base.GetAsync(carbonContextTestClassList.FirstOrDefault().TenantId, x => x.Name == "Test Name 1");

            // Assert
            Assert.Equal("Test Name 1", response.Name);
        }

        [Theory]
        [CreateRangeEntityEFRepository]
        public async Task Query_Successfully_ReturnEntity(List<CarbonContextTestClass> carbonContextTestClassList)
        {
            // Arrange
            var createResponse = await base.CreateRangeAsync(carbonContextTestClassList);

            // Act
            var response = base.Query(carbonContextTestClassList.FirstOrDefault().TenantId).ToList();

            // Assert
            Assert.IsType<List<CarbonContextTestClass>>(response);
        }

        [Theory]
        [CreateRangeEntityEFRepository]
        public async Task QueryAsNoTracking_Successfully_ReturnEntity(List<CarbonContextTestClass> carbonContextTestClassList)
        {
            // Arrange
            var createResponse = await base.CreateRangeAsync(carbonContextTestClassList);

            // Act
            var response = base.QueryAsNoTracking(carbonContextTestClassList.FirstOrDefault().TenantId).ToList();

            // Assert
            Assert.IsType<List<CarbonContextTestClass>>(response);
        }

        [Theory]
        [CreateRangeEntityEFRepository]
        public async Task SaveChangesAsync_Successfully_ReturnInteger(List<CarbonContextTestClass> carbonContextTestClassList)
        {
            // Arrange
            var createResponse = await base.CreateRangeAsync(carbonContextTestClassList);
            base.context.CarbonContextTestClass.Add(new CarbonContextTestClass { Id = Guid.NewGuid(), Name = "Name 3" });
            // Act
            var response = await base.SaveChangesAsync();

            // Assert
            Assert.IsType<int>(response);
        }
    }
}
