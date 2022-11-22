using StackExchange.Redis;
using StackExchange.Redis.Profiling;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Carbon.Redis
{
    /// <summary>
    /// If redis  disabled DummyConnectionMultiplexer injected
    /// </summary>
    public class DummyConnectionMultiplexer : IConnectionMultiplexer
    {
        public string ClientName  { get; set; }

        public string Configuration  { get; set; }

        public int TimeoutMilliseconds =>  default;

        public long OperationCount =>  default;

        public bool PreserveAsyncOrder { get; set; }

        public bool IsConnected =>  default;

        public bool IsConnecting =>  default;

        public bool IncludeDetailInExceptions { get; set; }
        public int StormLogThreshold { get; set; }

        public event EventHandler<RedisErrorEventArgs> ErrorMessage;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<InternalErrorEventArgs> InternalError;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored;
        public event EventHandler<EndPointEventArgs> ConfigurationChanged;
        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast;
        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved;

        public void Close(bool allowCommandsToComplete = true)
        {

        }

        public Task CloseAsync(bool allowCommandsToComplete = true)
        {
            return default;
        }

        public bool Configure(TextWriter log = null)
        {
            return default;
        }

        public Task<bool> ConfigureAsync(TextWriter log = null)
        {
            return default;
        }

        public void Dispose()
        {

        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public void ExportConfiguration(Stream destination, ExportOptions options = (ExportOptions)(-1))
        {

        }

        public ServerCounters GetCounters()
        {
            return default;
        }

        public IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            return new RedisDatabase();
        }

        public virtual EndPoint[] GetEndPoints(bool configuredOnly = false)
        {
            return new EndPoint[1];
        }

        public int GetHashSlot(RedisKey key)
        {
            return default;
        }

        public IServer GetServer(string host, int port, object asyncState = null)
        {
            return  new DummyRedisServer();
        }

        public IServer GetServer(string hostAndPort, object asyncState = null)
        {
            return  new DummyRedisServer();
        }

        public IServer GetServer(IPAddress host, int port)
        {
            return  new DummyRedisServer();
        }

        public IServer GetServer(EndPoint endpoint, object asyncState = null)
        {
            return new DummyRedisServer();
        }

        public IServer[] GetServers()
        {
            throw new NotImplementedException();
        }

        public string GetStatus()
        {
            return default;
        }

        public void GetStatus(TextWriter log)
        {

        }

        public string GetStormLog()
        {
            return default;
        }

        public ISubscriber GetSubscriber(object asyncState = null)
        {
            return default;
        }

        public int HashSlot(RedisKey key)
        {
            return default;
        }

        public long PublishReconfigure(CommandFlags flags = CommandFlags.None)
        {
            return default;
        }

        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None)
        {
            return default;
        }

        public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider)
        {
           
        }

        public void ResetStormLog()
        {

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
    }
}
