using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace SharedLibraries.EventBus
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly string _exchangeName;
        private readonly Dictionary<string, List<Func<object, Task>>> _handlers;
        private readonly Dictionary<Type, string> _eventNameMap;

        public RabbitMQEventBus(IOptions<RabbitMQOptions> options, ILogger<RabbitMQEventBus> logger)
        {
            _logger = logger;
            _exchangeName = options.Value.ExchangeName;
            _handlers = new Dictionary<string, List<Func<object, Task>>>();
            _eventNameMap = new Dictionary<Type, string>();

            var factory = new ConnectionFactory()
            {
                HostName = options.Value.HostName,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password,
                VirtualHost = options.Value.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);
        }

        public async Task PublishAsync<T>(T eventData, string? routingKey = null) where T : class
        {
            var eventName = routingKey ?? GetEventName<T>();
            var message = JsonSerializer.Serialize(eventData);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Type = typeof(T).Name;

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: eventName,
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("Published event {EventName} with data: {EventData}", eventName, message);
            await Task.CompletedTask;
        }

        public async Task SubscribeAsync<T>(Func<T, Task> handler, string? routingKey = null) where T : class
        {
            Subscribe(handler, routingKey);
            await Task.CompletedTask;
        }

        public void Subscribe<T>(Func<T, Task> handler, string? routingKey = null) where T : class
        {
            var eventName = routingKey ?? GetEventName<T>();
            var queueName = $"{eventName}_queue";

            // Declare queue
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: eventName);

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers[eventName] = new List<Func<object, Task>>();
            }

            _handlers[eventName].Add(async (eventData) =>
            {
                if (eventData is T typedEventData)
                {
                    await handler(typedEventData);
                }
                else
                {
                    _logger.LogWarning("Event data type mismatch for event {EventName}", eventName);
                }
            });

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var eventData = JsonSerializer.Deserialize<T>(message);

                    if (eventData != null && _handlers.ContainsKey(eventName))
                    {
                        foreach (var handlerFunc in _handlers[eventName])
                        {
                            await handlerFunc(eventData);
                        }
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Processed event {EventName} with data: {EventData}", eventName, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing event {EventName}", eventName);
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation("Subscribed to event {EventName} on queue {QueueName}", eventName, queueName);
        }

        public void Unsubscribe<T>() where T : class
        {
            var eventName = GetEventName<T>();
            if (_handlers.ContainsKey(eventName))
            {
                _handlers.Remove(eventName);
                _logger.LogInformation("Unsubscribed from event {EventName}", eventName);
            }
        }

        private string GetEventName<T>() where T : class
        {
            var type = typeof(T);
            if (_eventNameMap.ContainsKey(type))
            {
                return _eventNameMap[type];
            }

            var eventName = type.Name.Replace("Event", "").ToLowerInvariant();
            _eventNameMap[type] = eventName;
            return eventName;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }

    public class RabbitMQOptions
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string ExchangeName { get; set; } = "ecommerce_events";
    }
}
