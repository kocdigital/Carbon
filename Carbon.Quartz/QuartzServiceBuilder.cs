using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;

namespace Carbon.Quartz
{
    public static class QuartzServiceBuilder
    {
        public static void AddQuartzScheduler(this IServiceCollection services, IConfiguration configuration, bool isPersistent = true, string schedulerName = "NamelessScheduler", int maxConcurrency = 10)
        {
            if (maxConcurrency > 10)
                maxConcurrency = 10;
            // base configuration from appsettings.json

            services.Configure<QuartzOptions>(k => configuration.GetSection("Quartz"));

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
