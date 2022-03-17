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
        /// Migrates your Quartz Auto-Migrations (Postgre and MSSQL supported)
        /// </summary>
        /// <param name="app"></param>
        public static void MigrateQuartz(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<QuartzMigrationContext>();
                context.Database.Migrate();
            }
        }

        /// <summary>
        /// Manages Quartz Auto-Migrations as Multi Target. Use this if you are using quartz as persisted. 
        /// </summary>
        /// <typeparam name="TStartup">Your Startup Class</typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddQuartzContext<TStartup>(this IServiceCollection services, IConfiguration configuration)
            where TStartup : class
        {
            Console.WriteLine("Adding Quartz Context...");

            var connectionString = configuration.GetSection(QuartzContextConstants.Quartz).GetConnectionString(QuartzContextConstants.DefaultConnection);
            var target = configuration.GetSection(QuartzContextConstants.Quartz).GetConnectionString(QuartzContextConstants.ConnectionTarget);

            var migrationsAssembly = "Carbon.Quartz.Migrate." + target;
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            switch (target.ToLower())
            {
                case QuartzContextConstants.PostgreSQLLowerCase:
                    services.AddDbContext<QuartzMigrationContext>(options => options.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
                    break;
                case QuartzContextConstants.MSSQLLowerCase:
                    services.AddDbContext<QuartzMigrationContext>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
                    break;
                default:
                    throw new Exception("No Valid Connection Target Found");
            }
        }
    }
}
