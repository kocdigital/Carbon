using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using Carbon.TimeScaleDb;
using System.Runtime.Loader;
using System.Linq;
using Carbon.Domain.Abstractions.Entities;
using Carbon.TimeSeriesDb.Abstractions.Entities;
using Carbon.TimeSeriesDb.Abstractions.Attributes;

namespace Carbon.TimeScaleDb.EntityFrameworkCore
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Migrates your database with related to the configurations in AddDatabaseContext method
        /// </summary>
        /// <typeparam name="TContext">Your Database Context</typeparam>
        /// <param name="app"></param>
        public static void MigrateTimeScaleDatabase<TContext>(this IApplicationBuilder app) where TContext : CarbonTimeScaleDbContext<TContext>
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var tsdbHelper = serviceScope.ServiceProvider.GetRequiredService<ITimeScaleDbHelper>();
                if (!tsdbHelper.CheckTimeScaleDbSupport())
                {
                    throw new NotSupportedException("Database does not support TimeScaleDb");
                }

                var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();


                context.Database.Migrate();
                var timeSerieEntities = context.GetType().Assembly.GetTypes()
                    .Where(type => typeof(ITimeSeriesEntity).IsAssignableFrom(type) && !type.IsInterface)
                    .ToList();

                if (timeSerieEntities != null && timeSerieEntities.Any())
                    foreach (var tse in timeSerieEntities)
                    {
                        var timeSerieTaggedProperties = tse.GetProperties().Where(k => k.CustomAttributes.Where(k => k.AttributeType == typeof(TimeSerie)).Any()).ToList();
                        if (timeSerieTaggedProperties == null || !timeSerieTaggedProperties.Any())
                        {
                            throw new NotSupportedException("Database object does not contain any timeserie field! Remember to tag any of your DateTime Property with [TimeSerie] Attribute");
                        }
                        else if (timeSerieTaggedProperties.Count > 1)
                        {
                            throw new NotImplementedException("Database object contains more than 1 timeserie field! Remember to tag only one of your DateTime Property with [TimeSerie] Attribute");
                        }
                        var tsDbConversionSuccess = tsdbHelper.ConvertTableToTimeSeriesDb(tse.Name.ToLower(), timeSerieTaggedProperties[0].Name.ToLower());
                        if (tsDbConversionSuccess)
                            TimeSeriesTableInfo.TableTimeSeriePair.Add(tse.Name.ToLower(), timeSerieTaggedProperties[0].Name.ToLower());
                        else
                            throw new Exception("Neither Timescale DB migrated nor found! Please check your migration");
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
        public static void AddTimeScaleDatabaseContext<TContext, TStartup>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
            where TStartup : class
        {
            services.AddScoped<ITimeScaleDbHelper, TimeScaleDbHelper>();

            var connectionString = configuration.GetConnectionString("TimeScaleDbConnectionString");

            var migrationsAssembly = typeof(TStartup).GetTypeInfo().Assembly.GetName().Name + "." + "TimeScaleDb";
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            services.AddDbContext<TContext>(options => options.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
            services.AddTimeScaleDatabaseContextHealthCheck(connectionString);

            try
            {
                AssemblyLoadContext.Default.LoadFromAssemblyPath($"{path}\\{migrationsAssembly}.dll");
            }
            catch
            {
                Console.WriteLine("No Migration Assembly Loaded!");
            }

        }

        public static void AddTimeScaleDatabaseWithReadOnlyReplicaContext<TContext, RContext, TStartup>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
            where RContext : DbContext, ITimeScaleDbReadOnlyContext
            where TStartup : class
        {
            services.AddScoped<ITimeScaleDbHelper, TimeScaleDbHelper>();

            var connectionString = configuration.GetConnectionString("TimeScaleDbConnectionString");
            var readReplicaEnabled = configuration.GetConnectionString("ReadReplicaEnabled");

            var migrationsAssembly = typeof(TStartup).GetTypeInfo().Assembly.GetName().Name + "." + "TimeScaleDb";
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            services.AddDbContext<TContext>(options => options.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            services.AddTimeScaleDatabaseContextHealthCheck(connectionString);

            if (Boolean.TryParse(readReplicaEnabled, out bool rrEnabled) && rrEnabled)
            {
                var readOnlyConnectionString = configuration.GetConnectionString("TimeScaleDbReadOnlyConnectionString");
                if (String.IsNullOrEmpty(readOnlyConnectionString))
                    services.AddDbContext<RContext>(options => options.UseNpgsql(connectionString));
                else
                    services.AddDbContext<RContext>(options => options.UseNpgsql(readOnlyConnectionString));
            }
            else
                services.AddDbContext<RContext>(options => options.UseNpgsql(connectionString));

            try
            {
                AssemblyLoadContext.Default.LoadFromAssemblyPath($"{path}\\{migrationsAssembly}.dll");
            }
            catch
            {
                Console.WriteLine("No Migration Assembly Loaded!");
            }

        }

    }
}
