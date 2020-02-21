using System.Threading.Tasks;
using MassTransit;

namespace Carbon.Demo.ConsoleApplication
{
    public class TestConsumer : IConsumer<FlightOrder>
    {
        public Task Consume(ConsumeContext<FlightOrder> context)
        {

            System.Console.WriteLine($"Order processed: FlightId:{context.Message.FlightId} - OrderId:{context.Message.OrderId}");

            return Task.CompletedTask;
        }
    }
}
