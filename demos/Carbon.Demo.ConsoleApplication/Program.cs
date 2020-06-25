using Carbon.ConsoleApplication;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MassTransit;
using System;
using Microsoft.Extensions.DependencyInjection;
using Carbon.MassTransit;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Carbon.Common;

namespace Carbon.Demo.ConsoleApplication
{
    public interface ITestService
    {
        void ShowAlerts();
    }

    public class TestService : ITestService
    {
        private readonly ILogger<TestService> _logger;
        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }
        public void ShowAlerts()
        {
            _logger.LogTrace("Warning !");
            _logger.LogDebug("Debug !");
            _logger.LogInformation("Information !");
            _logger.LogWarning("Warning !");
            _logger.LogError("Error !");
            _logger.LogCritical("Critical !");
        }
    }

    public class MyHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private ITestService _testService;

        public MyHostedService(ITestService testService)
        {
            _testService = testService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Console.WriteLine("Timed Background Service is working.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Timed Background Service is stopping.2");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _testService.ShowAlerts();
            _timer?.Dispose();
        }
    }

    public class SortObject
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public double Points { get; set; }
    }

    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Hello World");

            List<SortObject> petList = new List<SortObject>();
            petList.Add(new SortObject { Name = "Barley", Number = 8, Points = 3.7 });
            petList.Add(new SortObject { Name = "Whiskers", Number = 1, Points = 3.13 });
            petList.Add(new SortObject { Name = "Boots", Number = 1, Points = 3.1 });
            IQueryable<SortObject> pets = petList.AsQueryable();

            var orderables = new List<Orderable>();
            orderables.Add(new Orderable
            {
                Value = "Name",
                IsAscending = true
            });
            orderables.Add(new Orderable
            {
                Value = "Number",
                IsAscending = false
            });

            var query = pets.OrderBy(orderables);

            foreach (var pet in query)
            {
                Console.WriteLine("{0} - {1} - {2}", pet.Name, pet.Number, pet.Points);
            }

            /*var host = new HostBuilder()
                    .AddCarbonServices<Program>((hostContext, services) =>
                    {
                        services.AddTransient<ITestService, TestService>();
                        services.AddMassTransitBus(cfg =>
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

                        services.AddHostedService<MyHostedService>();
                        services.AddHostedService<MyQueryableTestService>();

                    })
                    .UseServiceProvider((serviceProvider) =>
                    {
                        
                    });

            await host.RunConsoleAsync(); */
        }
    }
}
