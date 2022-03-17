using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;

namespace Carbon.Quartz
{
    public static class QuartzServiceBuilder
    {
        /// <summary>
        /// Adds a quartz scheduler which is persistable and clusterable
        /// </summary>
        /// <param name="services">Your service collection</param>
        /// <param name="configuration">Your Configuration</param>
        /// <param name="isPersistent">If persistent, use Carbon.Quartz.Migrate packages to make quartz migrate database schema automatically, otherwise you need to manually create all the db and tables</param>
        /// <param name="schedulerName">Give scheduler a name, and use this name while adding a job to quartz</param>
        /// <param name="maxConcurrency">Max parallel job for your scheduled tasks (max:10)</param>
        /// <exception cref="NotSupportedException"></exception>
        public static void AddQuartzScheduler(this IServiceCollection services, IConfiguration configuration, bool isPersistent = true, string schedulerName = "NamelessScheduler", int maxConcurrency = 10)
        {
            if (maxConcurrency > 10)
                maxConcurrency = 10;
            // base configuration from appsettings.json

            services.Configure<QuartzOptions>(k => configuration.GetSection(QuartzConstants.Quartz));

            // if you are using persistent job store, you might want to alter some options
            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = true; // default: true
                options.Scheduling.OverWriteExistingData = true; // default: true
            });

            services.AddQuartz(q =>
            {
                // handy when part of cluster or you want to otherwise identify multiple schedulers
                q.SchedulerId = QuartzConstants.SchedulerAutoId;
                // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseDefaultThreadPool(x => x.MaxConcurrency = maxConcurrency);
                q.SchedulerName = schedulerName;
                if (!isPersistent)
                {
                    q.UseInMemoryStore();
                }
                else
                {
                    var connString = configuration.GetSection(QuartzConstants.Quartz).GetConnectionString(QuartzConstants.DefaultConnection);
                    var target = configuration.GetSection(QuartzConstants.Quartz).GetConnectionString(QuartzConstants.ConnectionTarget);
                    q.UsePersistentStore(s =>
                    {
                        s.UseProperties = true;
                        s.RetryInterval = TimeSpan.FromSeconds(15);
                        s.UseJsonSerializer();
                        if (target == QuartzConstants.PostgreSQL)
                        {
                            s.UsePostgres(sqlServer =>
                            {
                                sqlServer.ConnectionString = connString;
                            });
                        }
                        else if (target == QuartzConstants.MSSQL)
                        {
                            s.UseSqlServer(sqlServer =>
                            {
                                sqlServer.ConnectionString = connString;
                            });
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }

                        s.UseClustering(c =>
                        {
                            c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                            c.CheckinInterval = TimeSpan.FromSeconds(10);
                        });
                    });
                }

            });

            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
        }
    }
}
