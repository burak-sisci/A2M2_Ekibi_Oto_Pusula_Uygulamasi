using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace backend.API.Shared.Messaging;

public sealed class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
    {
        _logger = logger;
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING")
            ?? "amqp://guest:guest@localhost:5672/";

        var factory = new ConnectionFactory { Uri = new Uri(connectionString) };

        try
        {
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel    = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "RabbitMQ bağlantısı kurulamadı. Mesajlar atlanacak.");
            _connection = null!;
            _channel    = null!;
        }
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string queueName) where TEvent : class
    {
        if (_channel is null)
        {
            _logger.LogWarning("RabbitMQ kanalı mevcut değil. Event atlandı: {Queue}", queueName);
            return;
        }

        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        var props = new BasicProperties { Persistent = true };

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            basicProperties: props,
            body: body);

        _logger.LogInformation("RabbitMQ event yayınlandı: {Queue} — {EventType}", queueName, typeof(TEvent).Name);
    }

    public void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _channel?.DisposeAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
        _connection?.DisposeAsync().GetAwaiter().GetResult();
    }
}
