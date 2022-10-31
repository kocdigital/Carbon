using Microsoft.Extensions.Caching.Distributed;
using RedLockNet.SERedis;
using StackExchange.Redis;

namespace Carbon.Caching.Abstractions
{
    public interface ICarbonCache : IDistributedCache
    {
        IServer GetServer();
        int GetRedisDatabaseNumber();
        RedLockFactory GetRedLockFactory();
        IDatabase GetDatabase();
    }
}
