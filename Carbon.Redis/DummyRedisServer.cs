using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Carbon.Redis
{
    public class DummyRedisServer:IServer
    {
        public ClusterConfiguration ClusterConfiguration { get; }
        public EndPoint EndPoint { get; }
        public RedisFeatures Features { get; }
        public bool IsConnected { get; }
        public bool IsSlave { get; }
        public bool AllowSlaveWrites { get; set; }
        public ServerType ServerType { get; }
        public Version Version { get; }
        public int DatabaseCount { get; }
        public IConnectionMultiplexer Multiplexer { get; set; }
        public async Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
            return default;
        }

        public bool TryWait(Task task)
        {
           return default;
        }

        public void Wait(Task task)
        {
       
        }

        public T Wait<T>(Task<T> task)
        {
           return default;
        }

        public void WaitAll(params Task[] tasks)
        {
           
        }

        public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public void ClientKill(EndPoint endpoint, CommandFlags flags = CommandFlags.None)
        {
          
        }

        public async Task ClientKillAsync(EndPoint endpoint, CommandFlags flags = CommandFlags.None)
        {
          
        }

        public long ClientKill(long? id = null, ClientType? clientType = null, EndPoint endpoint = null, bool skipMe = true,
            CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<long> ClientKillAsync(long? id = null, ClientType? clientType = null, EndPoint endpoint = null, bool skipMe = true,
            CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public ClientInfo[] ClientList(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<ClientInfo[]> ClientListAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public ClusterConfiguration ClusterNodes(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<ClusterConfiguration> ClusterNodesAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public string ClusterNodesRaw(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<string> ClusterNodesRawAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public KeyValuePair<string, string>[] ConfigGet(RedisValue pattern = new RedisValue(), CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(RedisValue pattern = new RedisValue(), CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public void ConfigResetStatistics(CommandFlags flags = CommandFlags.None)
        {
          
        }

        public async Task ConfigResetStatisticsAsync(CommandFlags flags = CommandFlags.None)
        {
          
        }

        public void ConfigRewrite(CommandFlags flags = CommandFlags.None)
        {
           
        }

        public async Task ConfigRewriteAsync(CommandFlags flags = CommandFlags.None)
        {
           
        }

        public void ConfigSet(RedisValue setting, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
           
        }

        public async Task ConfigSetAsync(RedisValue setting, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
           
        }

        public long DatabaseSize(int database = 0, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<long> DatabaseSizeAsync(int database = 0, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public RedisValue Echo(RedisValue message, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<RedisValue> EchoAsync(RedisValue message, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public RedisResult Execute(string command, params object[] args)
        {
           return default;
        }

        public RedisResult Execute(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<RedisResult> ExecuteAsync(string command, params object[] args)
        {
           return default;
        }

        public async Task<RedisResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public void FlushAllDatabases(CommandFlags flags = CommandFlags.None)
        {
          
        }

        public async Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None)
        {
          
        }

        public void FlushDatabase(int database = 0, CommandFlags flags = CommandFlags.None)
        {
           
        }

        public async Task FlushDatabaseAsync(int database = 0, CommandFlags flags = CommandFlags.None)
        {
          
        }

        public ServerCounters GetCounters()
        {
           return default;
        }

        public IGrouping<string, KeyValuePair<string, string>>[] Info(RedisValue section = new RedisValue(), CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<IGrouping<string, KeyValuePair<string, string>>[]> InfoAsync(RedisValue section = new RedisValue(), CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public string InfoRaw(RedisValue section = new RedisValue(), CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<string> InfoRawAsync(RedisValue section = new RedisValue(), CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public IEnumerable<RedisKey> Keys(int database, RedisValue pattern, int pageSize, CommandFlags flags)
        {
           return default;
        }

        public IEnumerable<RedisKey> Keys(int database = 0, RedisValue pattern = new RedisValue(), int pageSize = 250, long cursor = 0,
            int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async IAsyncEnumerable<RedisKey> KeysAsync(int database = 0, RedisValue pattern = new RedisValue(), int pageSize = 250, long cursor = 0,
            int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            yield return "key:";
        }

        public DateTime LastSave(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<DateTime> LastSaveAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public void MakeMaster(ReplicationChangeOptions options, TextWriter log = null)
        {
           
        }

        public void Save(SaveType type, CommandFlags flags = CommandFlags.None)
        {
         
        }

        public async Task SaveAsync(SaveType type, CommandFlags flags = CommandFlags.None)
        {
          
        }

        public bool ScriptExists(string script, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public bool ScriptExists(byte[] sha1, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<bool> ScriptExistsAsync(string script, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<bool> ScriptExistsAsync(byte[] sha1, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public void ScriptFlush(CommandFlags flags = CommandFlags.None)
        {
          
        }

        public async Task ScriptFlushAsync(CommandFlags flags = CommandFlags.None)
        {
           
        }

        public byte[] ScriptLoad(string script, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public LoadedLuaScript ScriptLoad(LuaScript script, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<byte[]> ScriptLoadAsync(string script, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<LoadedLuaScript> ScriptLoadAsync(LuaScript script, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public void Shutdown(ShutdownMode shutdownMode = ShutdownMode.Default, CommandFlags flags = CommandFlags.None)
        {
           
        }

        public void SlaveOf(EndPoint master, CommandFlags flags = CommandFlags.None)
        {
           
        }

        public async Task SlaveOfAsync(EndPoint master, CommandFlags flags = CommandFlags.None)
        {
           
        }

        public CommandTrace[] SlowlogGet(int count = 0, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<CommandTrace[]> SlowlogGetAsync(int count = 0, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public void SlowlogReset(CommandFlags flags = CommandFlags.None)
        {
          
        }

        public async Task SlowlogResetAsync(CommandFlags flags = CommandFlags.None)
        {
           
        }

        public RedisChannel[] SubscriptionChannels(RedisChannel pattern = new RedisChannel(), CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<RedisChannel[]> SubscriptionChannelsAsync(RedisChannel pattern = new RedisChannel(), CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public long SubscriptionPatternCount(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<long> SubscriptionPatternCountAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public long SubscriptionSubscriberCount(RedisChannel channel, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<long> SubscriptionSubscriberCountAsync(RedisChannel channel, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public void SwapDatabases(int first, int second, CommandFlags flags = CommandFlags.None)
        {
          
        }

        public async Task SwapDatabasesAsync(int first, int second, CommandFlags flags = CommandFlags.None)
        {
         
        }

        public DateTime Time(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<DateTime> TimeAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<string> LatencyDoctorAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public string LatencyDoctor(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<long> LatencyResetAsync(string[] eventNames = null, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public long LatencyReset(string[] eventNames = null, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<LatencyHistoryEntry[]> LatencyHistoryAsync(string eventName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public LatencyHistoryEntry[] LatencyHistory(string eventName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<LatencyLatestEntry[]> LatencyLatestAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public LatencyLatestEntry[] LatencyLatest(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<string> MemoryDoctorAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public string MemoryDoctor(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task MemoryPurgeAsync(CommandFlags flags = CommandFlags.None)
        {
          
        }

        public void MemoryPurge(CommandFlags flags = CommandFlags.None)
        {
         
        }

        public async Task<RedisResult> MemoryStatsAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public RedisResult MemoryStats(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<string> MemoryAllocatorStatsAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public string MemoryAllocatorStats(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public EndPoint SentinelGetMasterAddressByName(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<EndPoint> SentinelGetMasterAddressByNameAsync(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public EndPoint[] SentinelGetSentinelAddresses(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<EndPoint[]> SentinelGetSentinelAddressesAsync(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public KeyValuePair<string, string>[] SentinelMaster(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<KeyValuePair<string, string>[]> SentinelMasterAsync(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public KeyValuePair<string, string>[][] SentinelMasters(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<KeyValuePair<string, string>[][]> SentinelMastersAsync(CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public KeyValuePair<string, string>[][] SentinelSlaves(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<KeyValuePair<string, string>[][]> SentinelSlavesAsync(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public void SentinelFailover(string serviceName, CommandFlags flags = CommandFlags.None)
        {
          
        }

        public async Task SentinelFailoverAsync(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           
        }

        public KeyValuePair<string, string>[][] SentinelSentinels(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }

        public async Task<KeyValuePair<string, string>[][]> SentinelSentinelsAsync(string serviceName, CommandFlags flags = CommandFlags.None)
        {
           return default;
        }
    }
}
