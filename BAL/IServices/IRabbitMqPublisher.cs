namespace finsyncapi.BAL.IServices
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(T message, string queueName);
    }
}
