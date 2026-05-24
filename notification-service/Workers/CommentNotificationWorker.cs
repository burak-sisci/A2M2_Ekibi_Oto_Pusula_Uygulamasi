using System.Text;
using System.Text.Json;
using NotificationService.Models;
using NotificationService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Workers;

/// <summary>
/// `yorum.olusturuldu` queue'sunu tüketir.
/// Yorumun yapıldığı ilanın sahibini bulur ve cihazlarına FCM push gönderir.
/// </summary>
public class CommentNotificationWorker : BackgroundService
{
    private readonly ILogger<CommentNotificationWorker> _logger;
    private readonly RabbitMqConnection _rabbit;
    private readonly FcmSender   _fcm;
    private readonly MongoLookup _mongo;
    private IChannel? _channel;
    private const string QueueName = "yorum.olusturuldu";

    public CommentNotificationWorker(
        ILogger<CommentNotificationWorker> logger,
        RabbitMqConnection rabbit,
        FcmSender fcm,
        MongoLookup mongo)
    {
        _logger = logger;
        _rabbit = rabbit;
        _fcm    = fcm;
        _mongo  = mongo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _rabbit.Connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(
            queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null,
            cancellationToken: stoppingToken);
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) => await HandleAsync(ea, stoppingToken);

        await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("[Comment] {Queue} kuyruğunu dinliyor...", QueueName);

        // Hayatta kal
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleAsync(BasicDeliverEventArgs ea, CancellationToken ct)
    {
        try
        {
            var json = Encoding.UTF8.GetString(ea.Body.Span);
            var evt = JsonSerializer.Deserialize<CommentCreatedEvent>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (evt is null)
            {
                _logger.LogWarning("[Comment] JSON deserialize başarısız: {Json}", json);
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, ct);
                return;
            }

            _logger.LogInformation("[Comment] Event alındı: YorumId={YorumId} IlanId={IlanId} YazarId={YazarId}",
                evt.YorumId, evt.IlanId, evt.YazarId);

            var car = await _mongo.GetCarOwnerAsync(evt.IlanId, ct);
            if (car is null || string.IsNullOrEmpty(car.OwnerId))
            {
                _logger.LogWarning("[Comment] İlan/sahip bulunamadı: {IlanId}", evt.IlanId);
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, ct);
                return;
            }

            // Self-comment: kullanıcı kendi ilanına yorum yapmışsa atla
            if (car.OwnerId == evt.YazarId)
            {
                _logger.LogInformation("[Comment] Self-comment, bildirim atlandı.");
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, ct);
                return;
            }

            var devices = await _mongo.GetUserDevicesAsync(car.OwnerId, ct);
            if (devices.Count == 0)
            {
                _logger.LogInformation("[Comment] İlan sahibinin kayıtlı cihazı yok: ownerId={OwnerId}", car.OwnerId);
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, ct);
                return;
            }

            var authorName = await _mongo.GetUserNameAsync(evt.YazarId, ct) ?? "Bir kullanıcı";
            var carTitle = $"{car.Brand} {car.Model}".Trim();
            if (string.IsNullOrWhiteSpace(carTitle)) carTitle = "ilanınız";

            var title = $"Yeni yorum: {carTitle}";
            var body  = $"{authorName} ilanınıza yorum yaptı.";

            var data = new Dictionary<string, string>
            {
                ["type"]      = "comment.created",
                ["commentId"] = evt.YorumId,
                ["carId"]     = evt.IlanId
            };

            int sent = 0, failed = 0, invalid = 0;
            foreach (var d in devices)
            {
                var result = await _fcm.SendAsync(d.FcmToken, title, body, data, ct);
                switch (result)
                {
                    case FcmSendResult.Success: sent++; break;
                    case FcmSendResult.InvalidToken:
                        invalid++;
                        await _mongo.DeleteDeviceByTokenAsync(d.FcmToken, ct);
                        break;
                    default: failed++; break;
                }
            }

            _logger.LogInformation("[Comment] Bildirim sonucu: ✅{Sent}  ❌{Failed}  🗑{Invalid}", sent, failed, invalid);
            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Comment] Mesaj işleme hatası, DLQ'ya nack ediliyor.");
            // Tekrar denemesi için requeue=false (kalıcı hata varsayımı)
            try { await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, ct); } catch { }
        }
    }
}
