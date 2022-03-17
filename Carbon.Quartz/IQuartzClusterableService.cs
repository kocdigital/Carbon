using Quartz;
using System.Threading.Tasks;

namespace Carbon.Quartz
{
    public interface IQuartzClusterableService
    {
        Task AddAndStartClusterableBasicJob<TJob>(string jobName, object jobData, int secondsInterval)
            where TJob : IJob;
        void SetSchedulerId(string Id);
    }
}
