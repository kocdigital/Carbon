using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MassTransit;
using System;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Carbon.Demo.ConsoleApplication
{
    public class MassTransitConsoleHostedService : IHostedService
    {
        readonly IBusControl _bus;

        public MassTransitConsoleHostedService(IBusControl bus, ILoggerFactory loggerFactory)
        {
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);

            var _random = new Random();

            while (_random.Next(1, 999) != 100)
            {
                await _bus.Publish<FlightOrder>(new FlightOrder { FlightId = Guid.NewGuid(), OrderId = _random.Next(1, 1000) });
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _bus.StopAsync(cancellationToken);
        }
    }
}
