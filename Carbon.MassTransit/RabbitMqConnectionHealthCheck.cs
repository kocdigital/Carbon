using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.MassTransit
{
    /// <summary>
    /// Checks the RabbitMQ broker connectivity by opening (and re-using) a connection and creating a channel.
    /// </summary>
    /// <remarks>
    /// A single connection is kept per health check registration and re-created when it is closed,
    /// so the check does not open a new broker connection on every health probe.
    /// </remarks>
    public sealed class RabbitMqConnectionHealthCheck : IHealthCheck, IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        private IConnection _connection;

        public RabbitMqConnectionHealthCheck(Uri connectionUri)
        {
            if (connectionUri == null)
                throw new ArgumentNullException(nameof(connectionUri));

            _connectionFactory = new ConnectionFactory
            {
                Uri = connectionUri,
                AutomaticRecoveryEnabled = true
            };
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var connection = await GetConnectionAsync(cancellationToken).ConfigureAwait(false);
                using (var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    await channel.CloseAsync(cancellationToken).ConfigureAwait(false);
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }

        private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = _connection;
            if (connection != null && connection.IsOpen)
                return connection;

            await _connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_connection != null && _connection.IsOpen)
                    return _connection;

                _connection?.Dispose();
                _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _connectionLock.Dispose();
        }
    }
}
