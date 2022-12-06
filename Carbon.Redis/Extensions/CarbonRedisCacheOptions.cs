using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace Carbon.Caching.Redis
{
    public class CarbonRedisCacheOptions : RedisCacheOptions, IOptions<CarbonRedisCacheOptions>
    {
        public CarbonRedisCacheOptions()
            : base()
        {

        }
        CarbonRedisCacheOptions IOptions<CarbonRedisCacheOptions>.Value
        {
            get => this;
        }

        public bool EnablePubSub { get; set; }
    }

}
