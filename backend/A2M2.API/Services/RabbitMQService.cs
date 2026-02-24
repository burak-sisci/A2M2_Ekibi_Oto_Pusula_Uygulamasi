using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace A2M2.API.Services;

/// <summary>
/// RabbitMQ servisi — Publish + RPC Pattern
/// </summary>
public class RabbitMQService : IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly string _replyQueueName;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new();

    public const string PricePredictionQueue = "price_prediction_queue";

    public RabbitMQService(IConfiguration config)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(config["RabbitMQ:Port"] ?? "5672"),
                UserName = config["RabbitMQ:UserName"] ?? "guest",
                Password = config["RabbitMQ:Password"] ?? "guest",
                AutomaticRecoveryEnabled = true,
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Kuyrukları oluştur
            _channel.QueueDeclare(queue: PricePredictionQueue, durable: true, exclusive: false, autoDelete: false);

            // RPC yanıt kuyruğu
            _replyQueueName = _channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, ea) =>
            {
                var correlationId = ea.BasicProperties.CorrelationId;
                if (_callbackMapper.TryRemove(correlationId, out var tcs))
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    tcs.SetResult(body);
                }
            };

            _channel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: consumer);

            Console.WriteLine("RabbitMQ bağlantısı başarılı");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RabbitMQ bağlantı hatası: {ex.Message}");
            _replyQueueName = string.Empty;
        }
    }

    /// <summary>
    /// Mesaj yayınla
    /// </summary>
    public void Publish(string queue, object data)
    {
        if (_channel == null) throw new InvalidOperationException("RabbitMQ bağlantısı yok");

        var json = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: "", routingKey: queue, basicProperties: properties, body: body);
    }

    /// <summary>
    /// RPC pattern ile fiyat tahmini isteği gönder ve yanıt bekle
    /// </summary>
    public async Task<T?> RequestAsync<T>(string queue, object data, int timeoutMs = 30000)
    {
        if (_channel == null) throw new InvalidOperationException("RabbitMQ bağlantısı yok");

        var correlationId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<string>();
        _callbackMapper.TryAdd(correlationId, tcs);

        var properties = _channel.CreateBasicProperties();
        properties.CorrelationId = correlationId;
        properties.ReplyTo = _replyQueueName;
        properties.Persistent = true;

        var json = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(exchange: "", routingKey: queue, basicProperties: properties, body: body);

        // Timeout
        using var cts = new CancellationTokenSource(timeoutMs);
        cts.Token.Register(() =>
        {
            _callbackMapper.TryRemove(correlationId, out _);
            tcs.TrySetCanceled();
        });

        var response = await tcs.Task;
        return JsonSerializer.Deserialize<T>(response);
    }

    public bool IsConnected => _connection?.IsOpen ?? false;

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
