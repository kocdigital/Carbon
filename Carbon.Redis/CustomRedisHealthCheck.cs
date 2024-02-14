using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Carbon.Redis.Sentinel;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using StackExchange.Redis;
using StackExchange.Redis.Configuration;
using StackExchange.Redis.Maintenance;
using StackExchange.Redis.Profiling;

namespace Carbon.Redis
{
    public class CustomRedisHealthCheck : IHealthCheck
    {
        private readonly ConfigurationOptions _configurationOptions;
        private readonly IConnectionMultiplexer _connection;
        private readonly ISentinelConnectionMultiplexer _sentinelConnectionMultiplexer;
        private static readonly object _locker = new object();
        public CustomRedisHealthCheck(IConnectionMultiplexer connection, ConfigurationOptions configurationOptions, ISentinelConnectionMultiplexer sentinelConnectionMultiplexer)
        {
            _configurationOptions = configurationOptions;
            _connection = connection;
            _sentinelConnectionMultiplexer = sentinelConnectionMultiplexer;

        }
        /// <summary>
        /// Non-sentinel logic is based on https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.Redis/RedisHealthCheck.cs
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                bool isSentinel = _sentinelConnectionMultiplexer.IsSentinel;
                if (!isSentinel)
                {
                    List<IServer> serverList = new List<IServer>();
                    foreach (var endPoint in _connection.GetEndPoints(configuredOnly: true))
                    {
                        var server = _connection.GetServer(endPoint);
                        serverList.Add(server);
                        if (server.ServerType == ServerType.Cluster)
                        {
                            var clusterInfo = await server.ExecuteAsync("CLUSTER", "INFO");

                            if (clusterInfo is object && !clusterInfo.IsNull)
                            {
                                if (!clusterInfo.ToString()!.Contains("cluster_state:ok"))
                                {
                                    //cluster info is not ok!
                                    return new HealthCheckResult(context.Registration.FailureStatus, description: $"INFO CLUSTER is not on OK state for endpoint {endPoint}");
                                }
                            }
                            else
                            {
                                //cluster info cannot be read for this cluster node
                                return new HealthCheckResult(context.Registration.FailureStatus, description: $"INFO CLUSTER is null or can't be read for endpoint {endPoint}");
                            }
                        }
                        else
                        {
                            await _connection.GetDatabase().PingAsync();
                            await server.PingAsync();
                        }
                    }
                    return HealthCheckResult.Healthy($"Healthy Redis with given servers: {String.Join(",", serverList.Select(k => k.EndPoint.ToString()).ToList())}");
                }
                else
                {
                    var allSentinelServers = _sentinelConnectionMultiplexer.ConnectionMultiplexer.GetServers();
                    var allSentinelEndpoints = _connection.GetServers();
                    string serviceName = _configurationOptions.ServiceName;
                    EndPoint foundRunningMasterUrl = default;

                    List<Task> sentinelCheckTasks = new List<Task>();
                    foreach (var server in allSentinelServers)
                    {
                        sentinelCheckTasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                var masterAddressFoundByThisServer = server.SentinelGetMasterAddressByName(serviceName);
                                lock (_locker)
                                {
                                    foundRunningMasterUrl = masterAddressFoundByThisServer;
                                }
                            }
                            catch
                            {

                            }
                        }));
                    }
                    var degradedServers = new List<(IServer Server, bool timedOut, int latency)>();
                    foreach (var server in allSentinelEndpoints)
                    {
                        sentinelCheckTasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                var task = server.PingAsync();
                                if (await Task.WhenAny(task, Task.Delay(_configurationOptions.ConnectTimeout)) == task && !task.IsFaulted)
                                {
                                    // task completed within timeout
                                }
                                else
                                {
                                    var connected = server.IsConnected;
                                    degradedServers.Add((server, connected, connected ? (int)task?.Result.TotalMilliseconds : -1));
                                }
                            }
                            catch
                            {
                                degradedServers.Add((server, false, -1));
                            }
                        }));
                    }
                    await Task.WhenAll(sentinelCheckTasks);


                    if (foundRunningMasterUrl == default)
                    {
                        return new HealthCheckResult(context.Registration.FailureStatus, description: $"Unhealthy Redis Sentinel: No healthy master found in the given server set: {String.Join(",", allSentinelServers.Select(k => k.EndPoint.ToString()).ToList())}");
                    }
                    if (degradedServers.Any())
                    {
                        return HealthCheckResult.Degraded($"Degraded Redis Sentinel with given sentinel servers: " +
                            $"{String.Join(",", allSentinelServers.Select(k => k.EndPoint.ToString()).ToList())} and selected master is {foundRunningMasterUrl}" +
                            $" and degraded servers are: {String.Join(",", degradedServers.Select(k => k.Server.EndPoint.ToString()).ToList())}", null,
                            (new Dictionary<string, object>
                            {
                                {"Sentinels", allSentinelServers.Select(k => k.EndPoint.ToString()).ToList()},
                                {"Servers", allSentinelEndpoints.Select(k => k.EndPoint.ToString()).ToList()},
                                {"UnhealthyServers", degradedServers.Where(x => !x.timedOut).Select(k => k.Server.EndPoint.ToString()).ToList()},
                                {"TimedOutServers", degradedServers.Where(x => x.timedOut).Select(k => new LatencyContainer {Server= k.Server.EndPoint.ToString(), Latency=k.latency }).ToList()}
                            }));
                    }
                    return HealthCheckResult.Healthy($"Healthy Redis Sentinel with given sentinel servers: {String.Join(",", allSentinelServers.Select(k => k.EndPoint.ToString()).ToList())} and selected master is {foundRunningMasterUrl}");
                }

            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
class LatencyContainer
{
    public string Server { get; set; }
    public int Latency { get; set; }
}