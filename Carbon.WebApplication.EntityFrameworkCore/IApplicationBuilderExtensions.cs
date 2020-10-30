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

namespace Carbon.WebApplication.EntityFrameworkCore
{
    public static class IApplicationBuilderExtensions
    {
        public static void MigrateDatabase<TContext>(this IApplicationBuilder app) where TContext : DbContext
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
        public static void AddDatabaseContext<TContext, TStartup>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
            where TStartup : class
        {

            var target = configuration.GetConnectionString("ConnectionTarget");
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var migrationsAssembly = typeof(TStartup).GetTypeInfo().Assembly.GetName().Name + "." + target;
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            AssemblyLoadContext.Default.LoadFromAssemblyPath($"{path}\\{migrationsAssembly}.dll");

            services.AddDbContext<TContext>(options =>
            {
                switch (target.ToLower())
                {
                    case "postgresql":
                        options.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                        break;
                    case "mssql":
                        options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                        break;
                    case null:
                        options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                        break;
                    default:
                        throw new Exception("No Valid Connection Target Found");
                }
            });
        }
    }
}
