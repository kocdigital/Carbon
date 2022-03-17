using Newtonsoft.Json;
using Quartz;

namespace Carbon.Quartz
{
    public static class QuartzExtensions
    {
        /// <summary>
        /// Collect your default data from context that you have already passed in AddAndStartClusterableBasicJob
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T GetDefaultData<T>(this IJobExecutionContext context) where T : class
        {
            try
            {
                var data = context.JobDetail.JobDataMap.Get(QuartzConstants.DefaultData);
                if (data != null)
                {
                    var myConfig = JsonConvert.DeserializeObject<T>(data.ToString());
                    return myConfig;
                }

                return null;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
