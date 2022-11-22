using System.Collections.Generic;

namespace Carbon.Redis
{
    public interface IRedisSettings
    {
        /// <summary>
        /// redis server endpoints
        /// </summary>
        IList<string> EndPoints { get; set; }
        /// <summary>
        /// Default Redis Cache Key Length
        /// </summary>
        int KeyLength { get; set; }
        /// <summary>
        /// Redis is enabled or not
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// Time (seconds) at which to send a message to help keep sockets alive (60 sec default)
        /// </summary>
        int KeepAlive { get; set; }
        /// <summary>
        /// If true, Connect will not create a connection while no servers are available
        /// </summary>
        bool AbortOnConnectFail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool CommandMapAvailable { get; set; }
        /// <summary>
        /// Broadcast channel name for communicating configuration changes
        /// </summary>
        string ConfigurationChannel { get; set; }
        /// <summary>
        /// Key to use for selecting a server in an ambiguous master scenario
        /// </summary>
        string TieBreaker { get; set; }
        /// <summary>
        /// Time (seconds) to check configuration. This serves as a keep-alive for interactive sockets, if it is supported.
        /// </summary>
        int ConfigCheckSeconds { get; set; }
        /// <summary>
        /// "INFO", "CONFIG", "CLUSTER","PING", "ECHO", "CLIENT"
        /// </summary>
        HashSet<string> CommandMap { get; set; }
        /// <summary>
        /// Password for the redis server
        /// </summary>
        string Password { get; set; }
        /// <summary>
        /// Enables a range of commands that are considered risky
        /// </summary>
        bool AllowAdmin { get; set; }
        /// <summary>
        /// Time (ms) to allow for asynchronous operations
        /// </summary>
        int AsyncTimeout { get; set; }
        /// <summary>
        /// The number of times to repeat connect attempts during initial Connect
        /// </summary>
        int ConnectRetry { get; set; }
        /// <summary>
        /// Timeout (ms) for connect operations
        /// </summary>
        int ConnectTimeout { get; set; }
        /// <summary>
        /// Default database index, from 0 to databases - 1
        /// </summary>
        int DefaultDatabase { get; set; }
        /// <summary>
        /// Enable if redis port uses TLS
        /// </summary>
        bool SSLEnabled { get; set; }

        /// <summary>
        /// Sentinel Enablement
        /// </summary>
        public string SentinelServiceName { get; set; }
        public int SyncTimeout { get; set; }


    }
}
