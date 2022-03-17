using Quartz;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz.Impl;
using System;
using System.Linq;
using System.Collections.Generic;
using Quartz.Impl.Triggers;

namespace Carbon.Quartz
{
    public class QuartzService : IQuartzClusterableService
    {
        private IScheduler _scheduler;
        private string _schedulerId;
        public QuartzService()
        {

        }
        /// <summary>
        /// use SetSchedulerId method first identically same with your schedulername you gave in AddQuartzScheduler
        /// </summary>
        /// <param name="Id"></param>
        public void SetSchedulerId(string Id)
        {
            _schedulerId = Id;
        }

        private async Task<IScheduler> getRelatedScheduler()
        {
            if (String.IsNullOrEmpty(_schedulerId))
            {
                var listOfSchedulers = await DirectSchedulerFactory.Instance.GetAllSchedulers();
                if(listOfSchedulers == null || !listOfSchedulers.Any())
                    throw new KeyNotFoundException("No scheduler found");
                return listOfSchedulers.FirstOrDefault();
            }
            else
            {
                var foundscheduler = await DirectSchedulerFactory.Instance.GetScheduler(_schedulerId);
                if (foundscheduler == null)
                    throw new KeyNotFoundException("No scheduler found with Id: " + _scheduler);
                return foundscheduler;
            }
        }

        /// <summary>
        /// Adds a clusterable and persistable basic job to your scheduler. Before using this method, if your scheduler is persistable, then use SetSchedulerId method first identically same with your schedulername you gave in AddQuartzScheduler
        /// </summary>
        /// <typeparam name="TJob">You need to create a new job that inherits from IJob so that your job will be executed with the given second intervals</typeparam>
        /// <param name="jobName">Give a name to your job</param>
        /// <param name="jobData">Pass a data to use it in your execute context, if you don't have, pass string empty</param>
        /// <param name="secondsInterval">Tick time as second</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task AddAndStartClusterableBasicJob<TJob>(string jobName, object jobData, int secondsInterval)
            where TJob : IJob
        {
            try
            {
                _scheduler = await getRelatedScheduler();
                var configData = JsonConvert.SerializeObject(jobData);
                IJobDetail job = JobBuilder.Create<TJob>()
                                          .UsingJobData(QuartzConstants.DefaultData, configData)
                                          .StoreDurably()
                                          .WithIdentity(jobName)
                                          .Build();

                await _scheduler.AddJob(job, true);

                var triggerKey = new TriggerKey(jobName + "_trigger");
                var existingTrigger = await _scheduler.GetTrigger(triggerKey);
                ITrigger trigger = TriggerBuilder.Create()
                                                     .ForJob(job)
                                                     .WithIdentity(jobName + "_trigger")
                                                     .StartNow()
                                                     .WithSimpleSchedule(z => z.WithIntervalInSeconds(secondsInterval).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
                                                     .Build();

                if (existingTrigger != null)
                {
                    var implExistingTrigger = (SimpleTriggerImpl)existingTrigger;
                    if (implExistingTrigger.RepeatInterval.TotalSeconds != secondsInterval)
                        await _scheduler.RescheduleJob(triggerKey, trigger);
                }
                else
                {
                    await _scheduler.ScheduleJob(trigger);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
