using Carbon.ConsoleApplication;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Carbon.MassTransit;
using MassTransit;
using System;

namespace Carbon.Demo.ConsoleApplication
{
    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Hello World");

            var host = new HostBuilder().UseCarbonConfigureServices<Program>((hostContext, services) =>
            {
                services.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<TestConsumer>();
                });
            });

            await host.RunConsoleAsync();
        }
    }

    public class TestConsumer : IConsumer<FlightOrder>
    {
        public TestConsumer()
        {

        }

        public Task Consume(ConsumeContext<FlightOrder> context)
        {

            System.Console.WriteLine($"Order processed: FlightId:{context.Message.FlightId} - OrderId:{context.Message.OrderId}");

            return Task.CompletedTask;
        }
    }

    public class FlightOrder
    {
        public Guid FlightId { get; set; }
        public int OrderId { get; set; }
    }
}
