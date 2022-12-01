using Carbon.MassTransit;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Carbon.MassTransit
{
    public class ReceiveEndpointObserver : IReceiveEndpointObserver
    {
        private readonly IConfiguration _configuration;
        public ReceiveEndpointObserver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task Completed(ReceiveEndpointCompleted completed)
        {
            return Task.FromResult(true);
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            //queue-type based exceptions handled here
            if (faulted.Exception.InnerException is RabbitMQ.Client.Exceptions.OperationInterruptedException)
            {
                var fault = ((RabbitMQ.Client.Exceptions.OperationInterruptedException)faulted.Exception.InnerException);
                ushort faultCode = fault.ShutdownReason.ReplyCode;
                string faultMessage = fault.ShutdownReason.ReplyText;
                //If it is an existing queue with different type, delete it and let MassTransit create again
                if (faultCode == 406 && faultMessage.Contains("x-queue-type"))
                {
                    var massTransitSettings = _configuration.GetSection("MassTransit").Get<MassTransitSettings>();
                    var busSettings = massTransitSettings.RabbitMq;
                    var rabbitMqUql = $"amqp://{busSettings.Username}:{busSettings.Password}@{busSettings.Host}:{busSettings.Port}{busSettings.VirtualHost}";

                    var factory = new RabbitMQ.Client.ConnectionFactory()
                    {
                        Uri = new Uri(rabbitMqUql)
                    };
                    var absolutePath = faulted.ReceiveEndpoint.InputAddress.AbsolutePath.Split('/');
                    var queueName = absolutePath[absolutePath.Length - 1];
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                            channel.QueueDelete(queueName, false, false);
                    }
                }
            }
            return Task.FromResult(true);
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            return Task.FromResult(true);
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return Task.FromResult(true);
        }
    }
}
