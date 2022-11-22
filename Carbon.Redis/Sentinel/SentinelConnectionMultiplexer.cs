using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Redis.Sentinel
{
    public class SentinelConnectionMultiplexer : ISentinelConnectionMultiplexer
    {
        public SentinelConnectionMultiplexer(ConnectionMultiplexer connectionMultiplexer)
        {
            ConnectionMultiplexer = connectionMultiplexer;
            IsSentinel = true;
        }

        public ConnectionMultiplexer ConnectionMultiplexer { get; }
        public bool IsSentinel { get; }
    }

    public class NonSentinelConnectionMultiplexer : ISentinelConnectionMultiplexer
    {
        public NonSentinelConnectionMultiplexer(ConnectionMultiplexer connectionMultiplexer)
        {
            ConnectionMultiplexer = connectionMultiplexer;
            IsSentinel= false;
        }

        public ConnectionMultiplexer ConnectionMultiplexer { get; }
        public bool IsSentinel { get; }
    }
}
