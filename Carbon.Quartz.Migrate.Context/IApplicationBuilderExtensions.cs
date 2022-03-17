using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;
using System.IO;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Carbon.Domain.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Carbon.Quartz.Migrate.Context.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Migrates your database with related to the configurations in AddDatabaseContext method
        /// </summary>
        /// <typeparam name="TContext">Your Database Context</typeparam>
        /// <param name="app"></param>
        public static void MigrateQuartz<TContext>(this IApplicationBuilder app) where TContext : DbContext
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();
                context.Database.Migrate();
            }
        }

        /// <summary>
        /// Manages Multi EF Target Database Context and Discovers Desired Related Migration Assembly 
        /// </summary>
        /// <typeparam name="TContext">Your Database Context</typeparam>
        /// <typeparam name="TStartup">Your Startup Class</typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddQuartzContext<TContext, TStartup>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
            where TStartup : class
        {
            Console.WriteLine("Adding Quartz Context...");

            var connectionString = configuration.GetSection("Quartz").GetConnectionString("DefaultConnection");
            var target = configuration.GetSection("Quartz").GetConnectionString("ConnectionTarget");

            var migrationsAssembly = "Carbon.Quartz.Migrate." + target;
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            switch (target.ToLower())
            {
                case "postgresql":
                    services.AddDbContext<TContext>(options => options.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
                    break;
                case "mssql":
                    services.AddDbContext<TContext>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
                    break;
                default:
                    throw new Exception("No Valid Connection Target Found");
            }
        }
    }
}
