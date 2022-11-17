using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Carbon.Caching.Redis
{
    public class CarbonRedisCache : RedisCache, ICarbonRedisCache
    {
        private readonly RedLockFactory _redlockFactory; 
        private readonly IServer _redisServer;
        private ConnectionMultiplexer _existingConnectionMultiplexer;
        public delegate void OnExpiredHandler(string tagName);
        public static event OnExpiredHandler OnExpired;
        private readonly string _instanceName;
        public bool RegisterOnExpiredEventHandler(OnExpiredHandler handler)
        {
            if (handler == null)
            {
                return false;
            }

            if (OnExpired != null)
            {
                foreach (OnExpiredHandler existingHandler in OnExpired.GetInvocationList())
                {
                    if (existingHandler == handler)
                    {
                        return false;
                    }
                }
            }

            OnExpired += handler;
            return true;
        }

        public CarbonRedisCache(IOptions<CarbonRedisCacheOptions> optionsAccessor, IConnectionMultiplexer connectionMultiplexer) : base(optionsAccessor)
        {
            var _redisUrl = optionsAccessor.Value.Configuration;
            _instanceName = optionsAccessor.Value.InstanceName;
            _existingConnectionMultiplexer = (ConnectionMultiplexer)connectionMultiplexer;
            var multiplexers = new List<RedLockMultiplexer>
            {
                 _existingConnectionMultiplexer
            };
            _redlockFactory = RedLockFactory.Create(multiplexers);
            _redisServer = _existingConnectionMultiplexer.GetServer(_existingConnectionMultiplexer.GetEndPoints().FirstOrDefault());
            var subscriber = _existingConnectionMultiplexer.GetSubscriber();
            string keyspaceNotificationChannel = "__keyspace@" + _existingConnectionMultiplexer.GetDatabase().Database + "__:*";
            string keyeventNotificationChannel = "__keyevent@" + _existingConnectionMultiplexer.GetDatabase().Database + "__:*";
            subscriber.Subscribe(keyeventNotificationChannel, (channel, key) => //This needs CONFIG SET notify-keyspace-events Ex
            {
                var notificationType = GetKey(channel);
                switch (notificationType)
                {
                    case "expired": // requires the "Ex" keyspace notification options to be enabled
                        if (OnExpired != null)
                        {
                            OnExpired(key);
                        }
                        break;
                    default:
                        //("Unhandled notificationType: " + notificationType);
                        break;
                }
            });
        }

        public RedLockFactory GetRedLockFactory()
        {
            return _redlockFactory;
        }

        public IServer GetServer()
        {
            return _redisServer;
        }

        public int GetRedisDatabaseNumber()
        {
            return _existingConnectionMultiplexer.GetDatabase().Database;
        }

        private string GetKey(string channel)
        {
            var index = channel.IndexOf(':');
            if (index >= 0 && index < channel.Length - 1)
            {
                return channel.Substring(index + 1);
            }

            return channel;
        }

        public IDatabase GetDatabase()
        {
            return _existingConnectionMultiplexer.GetDatabase();
        }

        public string GetInstanceName()
        {
            return _instanceName;
        }
    }
}
