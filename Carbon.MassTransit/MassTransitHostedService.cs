using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.MassTransit
{
    /// <summary>
    /// MassTransitHostedService
    /// </summary>
    /// <remarks>
    ///  Defines methods for objects that are managed by the Mass Transit host.
    /// </remarks>
    public class MassTransitHostedService : IHostedService
    {
        readonly IBusControl _bus;

        /// <summary>
        /// Constructor that initializes an MassTransitHostedService with bus and loggerFactory
        /// </summary>
        /// <param name="bus">Mass Transit Bus Control</param>
        /// <param name="loggerFactory">ILogger instances creator logger factory</param>
        public MassTransitHostedService(IBusControl bus, ILoggerFactory loggerFactory)
        {
            _bus = bus;
        }

        /// <summary>
        /// Bus Async Start Method
        /// </summary>
        /// <remarks>
        /// Starts MassTransit Bus with cancellation token
        /// </remarks>
        /// <param name="cancellationToken">It enables cooperative cancellation between Tasks</param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);

        }

        /// <summary>
        /// Bus Async Stop Method
        /// </summary>
        /// <remarks>
        /// Stops MassTransit Bus with cancellation token
        /// </remarks>
        /// <param name="cancellationToken">It enables cooperative cancellation between Tasks</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _bus.StopAsync(cancellationToken);
        }
    }
}