using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Infrastructure.Persistence.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Messaging
{
    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private const string ExchangeName = "silentmoon.events";

        private readonly IAppLogger<RabbitMqEventPublisher> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqEventPublisher(
            IOptions<APIAppSettings> apiSettings,
            IAppLogger<RabbitMqEventPublisher> logger)
        {
            _logger = logger;
            var settings = apiSettings.Value.RabbitMq;

            var factory = new ConnectionFactory
            {
                HostName = settings.Host,
                Port = settings.Port,
                UserName = settings.Username,
                Password = settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Topic exchange: "reminder.created", "reminder.updated" kimi routing key-lər üçün
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
        }

        public Task PublishAsync<T>(string routingKey, T message) where T : class
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;                 // broker restart-da itməsin
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Event published: {RoutingKey}", routingKey);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
