using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Redis.Sentinel
{
    public interface ISentinelConnectionMultiplexer
    {
        ConnectionMultiplexer ConnectionMultiplexer { get; }
        Boolean IsSentinel { get; }
    }
}
