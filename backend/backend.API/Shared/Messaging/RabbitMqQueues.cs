namespace backend.API.Shared.Messaging;

public static class RabbitMqQueues
{
    public const string KullaniciKayit   = "kullanici.kayit";
    public const string IlanOlusturuldu  = "ilan.olusturuldu";
    public const string IlanFavorilendi  = "ilan.favorilendi";
    public const string YorumOlusturuldu = "yorum.olusturuldu";

    // Faz 2.5 — Notification Service tüketicisi tarafından dinlenir
    public const string EmailSend        = "email.send";
}

/// <summary>
/// Notification Service tarafından tüketilen email gönderim event'i.
/// </summary>
public record EmailSendEvent(
    string To,
    string TemplateId,
    Dictionary<string, string> Variables);
