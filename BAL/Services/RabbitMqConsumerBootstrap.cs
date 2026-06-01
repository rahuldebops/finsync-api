using finsyncapi.BAL.IServices;
using finsyncapi.Helper;
using finsyncapi.Models;

namespace finsyncapi.BAL.Services
{
    public class RabbitMqConsumerBootstrap
    {
        private readonly IRabbitMqConsumer _consumer;
        private readonly IHangfireService _hangfireService;

        public RabbitMqConsumerBootstrap(IRabbitMqConsumer consumer,IHangfireService hangfireService)
        {
            _consumer = consumer;
            _hangfireService = hangfireService;
        }

        public async Task StartAsync()
        {
            await _consumer.StartConsumerAsync<SendOtpEmailEvent>(QueueNames.SendOtpEmail,
                async message =>
                {
                    _hangfireService.EnqueueOtpEmailJob(message.Email,message.Otp);

                    await Task.CompletedTask;
                });
        }
    }
}
