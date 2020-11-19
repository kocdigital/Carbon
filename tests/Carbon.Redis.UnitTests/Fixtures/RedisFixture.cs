using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Carbon.Redis.UnitTests.StaticWrappers.RedisHelper;
using StackExchange.Redis;

namespace Carbon.Redis.UnitTests.Fixtures
{
    public class RedisFixture
    {
        public async IAsyncEnumerable<RedisKey> RedisKeys()
        {
            yield return new RedisKey();
            await Task.CompletedTask; // to make the compiler warning go away
        }

        public RedisHelperWrapper RedisHelperWrapper => new RedisHelperWrapper();

        public Task<(bool, string)> KeyValidResult = new Task<(bool, string)>(() => (true, ""));
        private static EndPoint endPoint { get; set; }


        public readonly EndPoint[] EndPoints = {
            endPoint = new IPEndPoint(IPAddress.Any, 1),
            endPoint = new DnsEndPoint("192.1.1.1",2)
        };

        public readonly IServer RedisServer = new DummyRedisServer()
        {
            Multiplexer = new DummyConnectionMultiplexer() { ClientName = "ClientName", Configuration = "Configuration", StormLogThreshold = 1, IncludeDetailInExceptions = true, PreserveAsyncOrder = true },
            AllowSlaveWrites = true
        };

        public readonly IDatabase RedisDatabase = new RedisDatabase()
        {
            Multiplexer = new DummyConnectionMultiplexer() { ClientName = "ClientName", Configuration = "Configuration", StormLogThreshold = 1, IncludeDetailInExceptions = true, PreserveAsyncOrder = true },
            Database = 1
        };

        public readonly IDatabase RedisDatabaseWithoutMultiplexer = new RedisDatabase()
        {
            Database = 1
        };

        public readonly IConnectionMultiplexer Multiplexer = new DummyConnectionMultiplexer()
        {
            ClientName = "ClientName",
            Configuration = "Configuration",
            StormLogThreshold = 1,
            IncludeDetailInExceptions = true,
            PreserveAsyncOrder = true
        };

        public object JsonData => "\"test\": \"data\"";

        public object InvalidJsonData => "test";

    }
}
