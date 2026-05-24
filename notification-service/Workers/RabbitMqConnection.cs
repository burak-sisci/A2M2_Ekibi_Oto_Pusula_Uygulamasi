using RabbitMQ.Client;

namespace NotificationService.Workers;

/// <summary>
/// Tek bir IConnection paylaşılan singleton — her worker kendi channel'ını açar.
/// </summary>
public class RabbitMqConnection : IAsyncDisposable
{
    public IConnection Connection { get; private set; } = default!;
    private readonly ILogger<RabbitMqConnection> _logger;
    private readonly string _connectionString;

    public RabbitMqConnection(ILogger<RabbitMqConnection> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING")
            ?? "amqp://guest:guest@localhost:5672/";
    }

    public async Task InitAsync()
    {
        var factory = new ConnectionFactory { Uri = new Uri(_connectionString) };
        Connection = await factory.CreateConnectionAsync("otopusula-notification");
        _logger.LogInformation("RabbitMQ bağlandı: {Host}", Connection.Endpoint.HostName);
    }

    public async ValueTask DisposeAsync()
    {
        if (Connection is not null)
        {
            await Connection.CloseAsync();
            await Connection.DisposeAsync();
        }
    }
}
