using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace rabbithole
{
    class RabbitEFService : IHostedService, IDisposable
    {

        private const string Queue = "rabbithole";
        private readonly EventSinkContext ctx;

        private readonly ILogger<RabbitEFService> logger;

        private readonly IApplicationLifetime lifetime;

        private RabbitConfig rabbitConfig;
        private IConnection rabbitConn;
        private IModel channel;

        private string tag;

        public RabbitEFService(IConfiguration configuration, ILogger<RabbitEFService> logger, IApplicationLifetime applifetime, EventSinkContext context)
        {
            rabbitConfig = new RabbitConfig();
            configuration.Bind("RabbitMQ", rabbitConfig);
            lifetime = applifetime;
            this.logger = logger;
            ctx = context;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return ListenAndStore();
        }

        public Task ListenAndStore()
        {
            var tries = 0;
            logger.LogInformation($"Connecting to broker {rabbitConfig.HostName} (vhost: {rabbitConfig.VirtualHost}).");
            do {
                try {
                    var factory = new ConnectionFactory()
                    {
                        HostName = rabbitConfig.HostName,
                        Password = rabbitConfig.Password,
                        UserName = rabbitConfig.UserName,
                        VirtualHost = rabbitConfig.VirtualHost,
                    };
                    
                    factory.Ssl.Enabled = rabbitConfig.Ssl.Enabled;
                    factory.Ssl.ServerName = rabbitConfig.Ssl.ServerName;

                    rabbitConn = factory.CreateConnection();
                    channel = rabbitConn.CreateModel();
                } catch (BrokerUnreachableException ex) {
                    logger.LogError($"Unable to connect to broker. {ex.Message}");
                    tries++;
                    if (tries >= 10) {
                        logger.LogCritical($"Unable to connect to broker after {tries} tries.");
                        lifetime.StopApplication();
                        return Task.FromException(ex);
                    }
                    Thread.Sleep(500*tries);
                }
            } while (channel == null);


            
            var queue = channel.QueueDeclare(Queue, durable: true, exclusive: false, autoDelete: false);
            foreach (var binding in rabbitConfig.Bindings)
            {
                channel.ExchangeDeclare(binding.Exchange, ExchangeType.Topic, durable: true, autoDelete: true);
                channel.QueueBind(queue: queue.QueueName,
                                exchange: binding.Exchange,
                                routingKey: binding.Topic);
                logger.LogInformation($"Subscribing to {binding.Exchange}:{binding.Topic}");
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnEventRecieved();
            
            tag = channel.BasicConsume(queue: Queue,
                                autoAck: true,
                                consumer: consumer);
            logger.LogInformation("Listening for messages.");
            return Task.CompletedTask;
        }

        private EventHandler<BasicDeliverEventArgs> OnEventRecieved()
        {
            return (model, ea) =>
            {
                var e = new Event()
                {
                    Recieved = DateTime.UtcNow,
                    Exchange = ea.Exchange,
                    RoutingKey = ea.RoutingKey,
                    Content = Encoding.UTF8.GetString(ea.Body),
                };

                try {
                    System.Json.JsonValue.Parse(e.Content);
                } catch (Exception ex) {
                    logger.LogError($"Unable to parse message from {e.Exchange}:{e.RoutingKey}.");
                    return;
                }

                logger.LogDebug($"Storing ({e.Recieved}, {e.Exchange}, {e.RoutingKey}, {e.Content} ) to database.");
                ctx.Events.Add(e);
                ctx.SaveChanges();
            };
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (channel != null) {
                channel.BasicCancel(tag);
            }
            return Task.CompletedTask;
        }

        private Task TearDown() {
            if (channel != null) {
                foreach (var binding in rabbitConfig.Bindings)
                {
                    
                    channel.QueueBind(queue: Queue,
                                    exchange: binding.Exchange,
                                    routingKey: binding.Topic);
                    channel.ExchangeDelete(binding.Exchange, false);
                    logger.LogInformation("Subscribing to {binding.Exchange}:{binding.Topic}");
                }
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (channel != null) {
                channel.Dispose();
            }
            if (rabbitConn != null) {
                rabbitConn.Dispose();
            }
        }
    }
}