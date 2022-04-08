using Quartz;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.Quartz
{
    public interface IQuartzClusterableService
    {
        Task AddAndStartClusterableBasicJob<TJob>(string jobName, object jobData, int secondsInterval)
            where TJob : IJob;
        void SetSchedulerId(string Id);

        Task ClearAllJobsExceptFor(List<string> excludingJobKeyList);
        Task AddAndStartClusterableCustomJob<TJob>(string jobName, object jobData, ITrigger trigger)
             where TJob : IJob;
    }
}
