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

namespace Carbon.WebApplication.EntityFrameworkCore
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Migrates your database with related to the configurations in AddDatabaseContext method
        /// </summary>
        /// <typeparam name="TContext">Your Database Context</typeparam>
        /// <param name="app"></param>
        public static void MigrateDatabase<TContext>(this IApplicationBuilder app, bool EnableLegacyTimestampBehavior = true) where TContext : DbContext
        {
            if (EnableLegacyTimestampBehavior)
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();
                context.Database.Migrate();
            }
        }


        /// <summary>
        /// Manages Database Seeding. To Disable, use "DisableSeeding = false" configuration. 
        /// </summary>
        /// <typeparam name="TContext">Your Database Context</typeparam>
        /// <typeparam name="TContextSeed">Your Seeding Context If exists in your project (IContextSeed)</typeparam>
        /// <param name="app"></param>
        public static void SeedDatabase<TContext, TContextSeed>(this IApplicationBuilder app)
            where TContext : DbContext
            where TContextSeed : IContextSeed
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<TContextSeed>>();
                try
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();
                    var seeder = serviceScope.ServiceProvider.GetRequiredService<IContextSeed>();
                    var config = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var seedDisabled = config.GetConnectionString("DisableSeeding");

                    if (String.IsNullOrEmpty(seedDisabled) || !Convert.ToBoolean(seedDisabled))
                    {
                        seeder.SeedAsync<TContextSeed>(context, logger).Wait();
                        logger.LogInformation("Database Seeded");
                    }
                    else
                    {
                        logger.LogInformation("Database Seed Disabled! Skipping!");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Unable to Seed Database: " + ex.Message);
                }
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

            var target = configuration.GetConnectionString("ConnectionTarget") ?? "MSSQL";
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var migrationsAssembly = typeof(TStartup).GetTypeInfo().Assembly.GetName().Name + "." + target;
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

            services.AddDatabaseContextHealthCheck(target, connectionString);

            try
            {
                AssemblyLoadContext.Default.LoadFromAssemblyPath($"{path}{Path.AltDirectorySeparatorChar}{migrationsAssembly}.dll");
            }
            catch
            {
                Console.WriteLine("No Migration Assembly Loaded!");
            }

        }
        /// <summary>
        /// Manages Multi EF Target Database Context and Discovers Desired Related Migration Assembly. Contains support for adding a read-only context corresponding to a read-only replica.
        /// </summary>
        /// <typeparam name="TContext">Your Database Context</typeparam>
        /// <typeparam name="RContext">Your ReadOnly Database Context</typeparam>
        /// <typeparam name="TStartup">Your Startup Class</typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDatabaseWithReadOnlyReplicaContext<TContext, RContext, TStartup>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
            where RContext : DbContext, IReadOnlyContext
            where TStartup : class
        {
            var target = configuration.GetConnectionString("ConnectionTarget") ?? "MSSQL";
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var readReplicaEnabled = configuration.GetConnectionString("ReadReplicaEnabled");

            var migrationsAssembly = typeof(TStartup).GetTypeInfo().Assembly.GetName().Name + "." + target;
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

            services.AddDatabaseContextHealthCheck(target, connectionString);


            if (Boolean.TryParse(readReplicaEnabled, out bool rrEnabled) && rrEnabled)
            {
                var readOnlyConnectionString = configuration.GetConnectionString("DefaultReadOnlyConnection");
                var realReadOnlyConnectionString = String.IsNullOrEmpty(readOnlyConnectionString) ? connectionString : readOnlyConnectionString;
                AddDbContextWithoutMigration<RContext>(services, target, realReadOnlyConnectionString);
            }
            else
            {
                AddDbContextWithoutMigration<RContext>(services, target, connectionString);
            }

            try
            {
                AssemblyLoadContext.Default.LoadFromAssemblyPath($"{path}{Path.AltDirectorySeparatorChar}{migrationsAssembly}.dll");
            }
            catch
            {
                Console.WriteLine("No Migration Assembly Loaded!");
            }

        }

        private static void AddDbContextWithoutMigration<RTContext>(IServiceCollection services, string target, string connStr)
                where RTContext : DbContext
        {
            switch (target.ToLower())
            {
                case "postgresql":
                    services.AddDbContext<RTContext>(options => options.UseNpgsql(connStr));
                    break;
                case "mssql":
                    services.AddDbContext<RTContext>(options => options.UseSqlServer(connStr));
                    break;
                default:
                    throw new Exception("No Valid Connection Target Found");
            }
        }



        /// <summary>
        /// Adds db health checks to <see cref="IHealthChecksBuilder"/>
        /// </summary>
        /// <remarks>
        /// <see cref="AddDatabaseContext"/> and <see cref="AddDatabaseWithReadOnlyReplicaContext"/> methods adds this automatically, you don't need to add this again
        /// </remarks>
        /// <param name="services"></param>
        /// <param name="target">MSSQL or PostgreSQL</param>
        /// <param name="connectionString"></param>
        /// <param name="failureStatus"></param>
        public static void AddDatabaseContextHealthCheck(this IServiceCollection services, string target, string connectionString, HealthStatus failureStatus = HealthStatus.Unhealthy)
        {
            switch (target.ToLower())
            {
                case "mssql":
                    services.AddHealthChecks().AddSqlServer(connectionString, failureStatus: failureStatus, name: $"{target}");
                    break;
                case "postgresql":
                    services.AddHealthChecks().AddNpgSql(connectionString, failureStatus: failureStatus, name: $"{target}");
                    break;
                default:
                    throw new Exception("No Valid Connection Target Found");
            }
        }
    }
}
