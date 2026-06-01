using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using finsyncapi.BAL.IServices;
using finsyncapi.Models;
using finsyncapi.Helper;

namespace finsyncapi.BAL.Services
{
    public class RabbitMqConsumer : IRabbitMqConsumer, IAsyncDisposable
    {
        private readonly AppSettingsModel _options;
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly IHostApplicationLifetime _lifetime;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqConsumer(
            IOptions<AppSettingsModel> options,
            ILogger<RabbitMqConsumer> logger,
            IHostApplicationLifetime lifetime)
        {
            _options = options.Value;
            _logger = logger;
            _lifetime = lifetime;
        }

        public async Task StartConsumerAsync<T>(string queueName, Func<T, Task> handler)
        {
            var cancellationToken = _lifetime.ApplicationStopping;

            _logger.LogInformation("Consumer starting");

            var factory = new ConnectionFactory
            {
                HostName = _options.RabbitMq.Connection.Host,
                Port = (int)_options.RabbitMq.Connection.Port,
                UserName = _options.RabbitMq.Connection.Username,
                Password = _options.RabbitMq.Connection.Password,
                VirtualHost = _options.RabbitMq.Connection.VirtualHost,
                Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = _options.RabbitMq.Connection.Host
                }
            };

            var delay = TimeSpan.FromSeconds(2);
            var maxDelay = TimeSpan.FromSeconds(30);
            int retryCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(cancellationToken);
                    _logger.LogInformation("RabbitMQ connected");

                    _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

                    await _channel.QueueDeclareAsync(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        cancellationToken: cancellationToken);
                    _logger.LogInformation("Queue declared");

                    var consumer = new AsyncEventingBasicConsumer(_channel);

                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body.ToArray();
                            var json = Encoding.UTF8.GetString(body);

                            var message = JsonSerializer.Deserialize<T>(json);

                            if (message is not null)
                            {
                                await handler(message);
                            }

                            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing RabbitMQ message from queue {QueueName}", queueName);
                        }
                    };

                    await _channel.BasicConsumeAsync(
                        queue: queueName,
                        autoAck: false,
                        consumer: consumer,
                        cancellationToken: cancellationToken);

                    _logger.LogInformation("Consumer registered");
                    break;
                }
                catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                {
                    retryCount++;
                    _logger.LogError(ex, "Connection failures (attempt {RetryCount}). Retrying in {DelaySeconds}s...", retryCount, delay.TotalSeconds);
                    try
                    {
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, maxDelay.TotalSeconds));
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogInformation("Consumer stopped");

            try
            {
                if (_channel is not null)
                {
                    await _channel.CloseAsync();
                    _channel.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RabbitMQ channel");
            }

            try
            {
                if (_connection is not null)
                {
                    await _connection.CloseAsync();
                    _connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RabbitMQ connection");
            }
        }
    }
}