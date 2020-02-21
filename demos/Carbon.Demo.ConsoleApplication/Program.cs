using Carbon.ConsoleApplication;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MassTransit;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.Demo.ConsoleApplication
{
    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Hello World");

            var host = new HostBuilder()
                    .UseCarbonConfigureServices<Program>((hostContext, services) =>
                    {
                        services.AddMassTransit(cfg =>
                        {
                            cfg.AddConsumer<TestConsumer>();
                        });

                        services.AddSingleton<IHostedService, MassTransitConsoleHostedService>();
                    });

            await host.RunConsoleAsync();
        }
    }
}
