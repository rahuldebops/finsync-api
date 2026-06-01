namespace finsyncapi.BAL.IServices
{
    public interface IRabbitMqConsumer
    {
        Task StartConsumerAsync<T>(string queueName, Func<T, Task> handler);
    }
}
