using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Carbon.Redis
{
    public class RedisSettings : IRedisSettings, IOptions<RedisSettings>
    {
        public RedisSettings()
        {
            DefaultDatabase = 0;
        }
        public IList<string> EndPoints { get; set; }
        public int KeyLength { get; set; }
        public bool Enabled { get; set; }
        public int KeepAlive { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public string ConfigurationChannel { get; set; }
        public string TieBreaker { get; set; }
        public int ConfigCheckSeconds { get; set; }
        public HashSet<string> CommandMap { get; set; }
        public string Password { get; set; }
        public bool AllowAdmin { get; set; }
        public bool CommandMapAvailable { get; set; }
        public int AsyncTimeout { get; set; }
        public int ConnectRetry { get; set; }
        public int ConnectTimeout { get; set; }
        public int DefaultDatabase { get; set; }
        public bool SSLEnabled { get; set; }
        public string InstanceName { get; set; }
        public string SentinelServiceName { get; set; }
        public int SyncTimeout { get; set; }
        public string User { get; set; }
        public RedisSettings Value => this;
    }
}
