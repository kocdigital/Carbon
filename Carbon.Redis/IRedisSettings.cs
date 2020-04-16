using System.Collections.Generic;

namespace Carbon.Redis
{
    public interface IRedisSettings
    {
        IList<string> EndPoints { get; set; }
        int KeyLength { get; set; }
        bool Enabled { get; set; }
        int KeepAlive { get; set; }
        bool AbortOnConnectFail { get; set; }
        bool CommandMapAvailable { get; set; }
        string ConfigurationChannel { get; set; }
        string TieBreaker { get; set; }
        int ConfigCheckSeconds { get; set; }
        HashSet<string> CommandMap { get; set; }
        string Password { get; set; }
        bool AllowAdmin { get; set; }
        int AsyncTimeout { get; set; }
        int ConnectRetry { get; set; }
        int ConnectTimeout { get; set; }
        int DefaultDatabase { get; set; }
    }
}
