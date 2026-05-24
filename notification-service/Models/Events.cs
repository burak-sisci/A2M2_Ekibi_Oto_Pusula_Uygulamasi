namespace NotificationService.Models;

/// <summary>
/// Backend `yorum.olusturuldu` kuyruğuna gönderdiği event ile birebir aynı şema.
/// </summary>
public record CommentCreatedEvent(
    string YorumId,
    string IlanId,
    string YazarId,
    DateTime OlusturulmaTarihi);

/// <summary>
/// Backend `email.send` kuyruğuna gönderir.
/// </summary>
public record EmailSendEvent(
    string To,
    string TemplateId,
    Dictionary<string, string> Variables);

/// <summary>
/// MongoDB'den okuduğumuz minimal cihaz kaydı.
/// </summary>
public record DeviceRecord(string UserId, string FcmToken, string Platform);
