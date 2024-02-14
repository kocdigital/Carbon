using Carbon.Caching.Abstractions;
using Carbon.Caching.Redis;

using System.Runtime.CompilerServices;

namespace TestApp.Application.Services
{
    public interface IRedisTestService
    {
        Task<string> WriteToRedis(short ttlSeconds = 0);
    }
    public class RedisTestService : IRedisTestService
    {
        private readonly ICarbonRedisCache _cache;

        public RedisTestService(ICarbonRedisCache cache)
        {
            _cache = cache;
        }

        public async Task<string> WriteToRedis(short ttlSeconds = 0)
        {
            var id = Guid.NewGuid().ToString();
            var key = "ae_test:" + id;
            await _cache.SetAsync(key, id, timeSpan: TimeSpan.FromSeconds(ttlSeconds));
            return key;
        }
    }
}
