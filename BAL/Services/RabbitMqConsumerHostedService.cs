using Microsoft.Extensions.Hosting;

namespace finsyncapi.BAL.Services
{
    public sealed class RabbitMqConsumerHostedService : BackgroundService
    {
        private readonly RabbitMqConsumerBootstrap _bootstrap;

        public RabbitMqConsumerHostedService(
            RabbitMqConsumerBootstrap bootstrap)
        {
            _bootstrap = bootstrap;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            await _bootstrap.StartAsync();
        }
    }
}
