using finsyncapi.BAL.IServices;
using finsyncapi.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace finsyncapi.BAL.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly AppSettingsModel _options;

        public RabbitMqPublisher(IOptions<AppSettingsModel> options)
        {
            _options = options.Value;
        }

        public async Task PublishAsync<T>(T message, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.RabbitMq.Connection.Host,
                Port = (int)_options.RabbitMq.Connection.Port,
                UserName = _options.RabbitMq.Connection.Username,
                Password = _options.RabbitMq.Connection.Password,
                VirtualHost = _options.RabbitMq.Connection.VirtualHost
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName,durable: true,exclusive: false,autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await channel.BasicPublishAsync(exchange: string.Empty,routingKey: queueName,
                body: body);
        }
    }
}
