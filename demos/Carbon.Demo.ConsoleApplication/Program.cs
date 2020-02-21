using Carbon.ConsoleApplication;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MassTransit;
using System;
using Microsoft.Extensions.DependencyInjection;
using Carbon.MassTransit;
using RabbitMQ.Client;

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
                            cfg.AddConsumer<FlightPurchasedConsumer>();

                            cfg.AddRabbitMqBus(hostContext.Configuration, (provider, busFactoryConfig) =>
                            {
                                string flightOrdersTopic = "flight-orders";

                                busFactoryConfig.ReceiveEndpoint(flightOrdersTopic, configurator =>
                                {
                                    configurator.ExchangeType = ExchangeType.Topic;
                                    configurator.Consumer<FlightPurchasedConsumer>(provider);
                                });
                            });

                            cfg.AddServiceBus(hostContext.Configuration, (provider, busFactoryConfig) =>
                            {
                                string flightOrdersTopic = "flight-orders";

                                string subscriptionName = "flight-subscriber";

                                //bu isimde bir topic oluşturuyor
                                busFactoryConfig.Message<FlightOrder>(m => { m.SetEntityName(flightOrdersTopic); });

                                // setup Azure topic consumer
                                //flight-subscriber isimli bir subscribeını flight-orders topici için oluşturuyor
                                busFactoryConfig.SubscriptionEndpoint<FlightOrder>(subscriptionName, configurator =>
                                {
                                    configurator.Consumer<FlightPurchasedConsumer>(provider);
                                });
                            });
                        });

                        services.AddSingleton<IHostedService, MassTransitConsoleHostedService>();
                    });

            await host.RunConsoleAsync();
        }
    }
}
