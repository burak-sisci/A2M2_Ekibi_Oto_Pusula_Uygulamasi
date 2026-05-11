namespace backend.API.Shared.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, string queueName) where TEvent : class;
}
