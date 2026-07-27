using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SilentMoon.Application.DTOs.Email;
using SilentMoon.Application.Features.Auth.Events;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Messaging
{
    public class OtpEmailConsumer : BackgroundService
    {
        private const string ExchangeName = "silentmoon.events";
        private const string QueueName = "otp.send.queue";
        private const string RoutingKey = "otp.send";

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAppLogger<OtpEmailConsumer> _logger;
        private readonly RabbitMqSettings _settings;

        private IConnection _connection;
        private IModel _channel;

        public OtpEmailConsumer(
            IServiceScopeFactory scopeFactory,
            IOptions<APIAppSettings> apiSettings,
            IAppLogger<OtpEmailConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _settings = apiSettings.Value.RabbitMq;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Connect();
                    Consume();
                    break; // qoşulma və consume uğurlu oldu, loop-dan çıx
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMQ-ya qoşulma alınmadı, 5 saniyədən sonra yenidən cəhd ediləcək");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        private void Connect()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(QueueName, ExchangeName, RoutingKey);
            _channel.BasicQos(0, 10, false);
        }

        private void Consume()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (sender, args) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Body.ToArray());
                    var otpEvent = JsonSerializer.Deserialize<OtpEmailEvent>(json);

                    using var scope = _scopeFactory.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var (subject, body) = BuildEmail(otpEvent);

                    await emailService.SendAsync(new EmailRequest
                    {
                        To = otpEvent.Email,
                        Subject = subject,
                        Body = body
                    });

                    _channel.BasicAck(args.DeliveryTag, multiple: false);
                    _logger.LogInformation("OTP email sent to {Email}", otpEvent.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process OTP email message, message discarded");
                    _channel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(QueueName, autoAck: false, consumer: consumer);
        }

        private static (string Subject, string Body) BuildEmail(OtpEmailEvent e)
        {
            return e.Purpose switch
            {
                OtpPurpose.Resend => (
                    "SilentMoon - Email Verification (Resend)",
                    $"<h3>Hello, {e.FirstName}!</h3><p>Your new verification code: <b>{e.OtpCode}</b></p><p>This code expires in 5 minutes.</p>"),
                _ => (
                    "SilentMoon - Email Verification",
                    $"<h3>Welcome, {e.FirstName}!</h3><p>Your verification code: <b>{e.OtpCode}</b></p><p>This code expires in 5 minutes.</p>")
            };
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}