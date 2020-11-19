using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.ComponentModel.Design;
using Xunit;

namespace Carbon.WebApplication.EntityFrameworkCore.UnitTests
{

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
      : base(options)
        { }
        public TestDbContext()
        {

        }
    }

    public class Startup
    {

    }


    public class IApplicationBuilderExtensionsTests
    {
        private IConfiguration CreateConfigDefault()
        {
            var mockConfSection = new Mock<IConfigurationSection>();
            string returns = null;
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "ConnectionTarget")]).Returns(returns);
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns("MQ");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            return mockConfiguration.Object;
        }
        private IConfiguration CreateConfigMS()
        {
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "ConnectionTarget")]).Returns("MSSQL");
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns("MQ");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            return mockConfiguration.Object;
        }
        private IConfiguration CreateConfigPqSQL()
        {
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "ConnectionTarget")]).Returns("PostgreSQL");
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns("PQ");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            return mockConfiguration.Object;
        }
        private IConfiguration CreateConfigUnKnown()
        {
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "ConnectionTarget")]).Returns("MariaDB");
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns("MDB");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            return mockConfiguration.Object;
        }


        [Fact]
        public void AddDatabaseContext_DefaultTarget()
        {
            var services = new ServiceCollection();
            var ms = CreateConfigDefault();
            services.AddDatabaseContext<TestDbContext, Startup>(ms);

            var provider = services.BuildServiceProvider();
            var service = (TestDbContext)provider.GetService(typeof(TestDbContext));

            Assert.NotNull(service);
            Assert.Equal(typeof(TestDbContext), service.GetType());
        }
        [Fact]
        public void AddDatabaseContext_MSSQL()
        {
            var services = new ServiceCollection();
            var ms = CreateConfigMS();
            services.AddDatabaseContext<TestDbContext, Startup>(ms);

            var provider = services.BuildServiceProvider();
            var service = (TestDbContext)provider.GetService(typeof(TestDbContext));

            Assert.NotNull(service);
            Assert.Equal(typeof(TestDbContext), service.GetType());
        }
        [Fact]
        public void AddDatabaseContext_PQSQL()
        {
            var services = new ServiceCollection();
            var ms = CreateConfigPqSQL();
            services.AddDatabaseContext<TestDbContext, Startup>(ms);

            var provider = services.BuildServiceProvider();
            var service = (TestDbContext)provider.GetService(typeof(TestDbContext));

            Assert.NotNull(service);
            Assert.Equal(typeof(TestDbContext), service.GetType());
        }
        [Fact]
        public void AddDatabaseContext_UnKnown()
        {
            var services = new ServiceCollection();
            var ms = CreateConfigUnKnown();

            Exception e = Assert.Throws<Exception>(() => services.AddDatabaseContext<TestDbContext, Startup>(ms));
            Assert.Equal("No Valid Connection Target Found", e.Message);
        }


        [Fact]
        public void MigrateDatabaseContext_InMemory()
        {
            var builder = new Mock<IApplicationBuilder>().Object;
            Assert.Throws<ArgumentNullException>(() => builder.MigrateDatabase<TestDbContext>());
        }
    }
}
