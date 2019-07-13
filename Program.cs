using System;
using Microsoft.Extensions.Configuration;
using System.Linq;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace rabbithole
{
    class Program
    {
        private const string Queue = "rabbithole";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var config = new ConfigurationBuilder()
                .Build();

            var rabbitConfig = new RabbitConfig();
            config.Bind("rabbitmq", rabbitConfig);

            var factory = new ConnectionFactory()
            {
                HostName = rabbitConfig.HostName,
                Password = rabbitConfig.Password,
                UserName = rabbitConfig.UserName,
                VirtualHost = rabbitConfig.VirtualHost
            };
            var connstr = config.GetConnectionString("events");
            using (var context = new EventSinkContext(connstr))
            {
                using (var rabbitConn = factory.CreateConnection())
                {
                    using (var channel = rabbitConn.CreateModel())
                    {
                        var queue = channel.QueueDeclare(Queue, true);
                        foreach (var binding in rabbitConfig.Bindings)
                        {
                            channel.QueueBind(queue: queue.QueueName,
                                            exchange: binding.Exchange,
                                            routingKey: binding.Topic);
                        }

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += OnEventRecieved(context);
                        channel.BasicConsume(queue: Queue,
                                            autoAck: true,
                                            consumer: consumer);
                        Console.ReadLine();
                    }
                }
            }
        }

        private static EventHandler<BasicDeliverEventArgs> OnEventRecieved(EventSinkContext ctx)
        {
            return (model, ea) =>
            {
                var e = new Event()
                {
                    Timestamp = DateTime.UtcNow,
                    Exchange = ea.Exchange,
                    RoutingKey = ea.RoutingKey,
                    Content = Encoding.UTF8.GetString(ea.Body),
                };

                //TODO validate Content is actual JSON
                //TODO Attempt to read Timestamp from event if available.

                ctx.Events.Add(e);
                ctx.SaveChanges();
            };
        }
    }
}
