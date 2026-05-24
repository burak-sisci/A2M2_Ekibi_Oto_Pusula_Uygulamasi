using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace NotificationService.Services;

/// <summary>
/// MailKit ile SMTP üzerinden mail gönderir. Gmail App Password ile çalışacak şekilde yapılandırıldı.
/// </summary>
public class SmtpSender
{
    private readonly ILogger<SmtpSender> _logger;
    private readonly string _host;
    private readonly int    _port;
    private readonly string _user;
    private readonly string _pass;
    private readonly string _from;

    public SmtpSender(ILogger<SmtpSender> logger)
    {
        _logger = logger;
        _host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
        _port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        _user = Environment.GetEnvironmentVariable("SMTP_USER") ?? "";
        _pass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? "";
        _from = Environment.GetEnvironmentVariable("SMTP_FROM") ?? _user;
    }

    public async Task<bool> SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_user) || string.IsNullOrEmpty(_pass))
        {
            _logger.LogWarning("SMTP credentials eksik (SMTP_USER/SMTP_PASS). Mail atlandı: {To}", to);
            return false;
        }

        try
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_from));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_user, _pass, ct);
            await client.SendAsync(msg, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("Mail gönderildi → {To} ({Subject})", to, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mail gönderim hatası → {To}", to);
            return false;
        }
    }
}
