using System.Text;
using System.Text.Json;
using NotificationService.Models;
using NotificationService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Workers;

/// <summary>
/// `email.send` queue'sunu tüketir, template'i doldurup SMTP ile mail gönderir.
/// </summary>
public class EmailWorker : BackgroundService
{
    private readonly ILogger<EmailWorker> _logger;
    private readonly RabbitMqConnection _rabbit;
    private readonly SmtpSender _smtp;
    private IChannel? _channel;
    private const string QueueName = "email.send";

    public EmailWorker(ILogger<EmailWorker> logger, RabbitMqConnection rabbit, SmtpSender smtp)
    {
        _logger = logger;
        _rabbit = rabbit;
        _smtp   = smtp;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _rabbit.Connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(
            queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null,
            cancellationToken: stoppingToken);
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 5, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) => await HandleAsync(ea, stoppingToken);

        await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("[Email] {Queue} kuyruğunu dinliyor...", QueueName);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleAsync(BasicDeliverEventArgs ea, CancellationToken ct)
    {
        try
        {
            var json = Encoding.UTF8.GetString(ea.Body.Span);
            var evt = JsonSerializer.Deserialize<EmailSendEvent>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (evt is null || string.IsNullOrEmpty(evt.To))
            {
                _logger.LogWarning("[Email] Geçersiz event: {Json}", json);
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, ct);
                return;
            }

            var (subject, htmlBody) = await RenderTemplateAsync(evt.TemplateId, evt.Variables ?? new());
            var ok = await _smtp.SendAsync(evt.To, subject, htmlBody, ct);

            if (ok)
            {
                _logger.LogInformation("[Email] Gönderildi → {To} ({Tpl})", evt.To, evt.TemplateId);
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, ct);
            }
            else
            {
                // Tekrar denenecek
                _logger.LogWarning("[Email] Başarısız, requeue: {To}", evt.To);
                await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Email] İşleme hatası");
            try { await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, ct); } catch { }
        }
    }

    private async Task<(string subject, string body)> RenderTemplateAsync(string templateId, Dictionary<string, string> vars)
    {
        string subject = "OtoPusula";
        string templatePath = templateId switch
        {
            "password-reset" => "Templates/password-reset.html",
            _                => "Templates/generic.html"
        };

        subject = templateId switch
        {
            "password-reset" => "OtoPusula — Şifre Sıfırlama",
            _                => "OtoPusula bildirimi"
        };

        if (!File.Exists(templatePath))
        {
            // Inline fallback
            var lines = vars.Select(kv => $"<p><b>{kv.Key}:</b> {kv.Value}</p>");
            return (subject, $"<h2>{subject}</h2>{string.Join("\n", lines)}");
        }

        var html = await File.ReadAllTextAsync(templatePath);
        foreach (var kv in vars)
            html = html.Replace("{{" + kv.Key + "}}", kv.Value);

        return (subject, html);
    }
}
