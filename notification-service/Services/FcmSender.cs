using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace NotificationService.Services;

/// <summary>
/// Firebase Admin SDK ile push notification gönderimi.
/// firebase-credentials.json yolu FIREBASE_CREDENTIALS_PATH env'inden okunur (default: ./firebase-credentials.json).
/// </summary>
public class FcmSender
{
    private readonly ILogger<FcmSender> _logger;
    private readonly bool _initialized;

    public FcmSender(ILogger<FcmSender> logger)
    {
        _logger = logger;
        var path = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_PATH") ?? "firebase-credentials.json";

        try
        {
            if (FirebaseApp.DefaultInstance is null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(path)
                });
            }
            _initialized = true;
            _logger.LogInformation("Firebase Admin SDK başlatıldı.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Firebase init başarısız ({Path}). Bildirim göndermeden geçilecek.", path);
            _initialized = false;
        }
    }

    public bool Initialized => _initialized;

    /// <summary>Tek cihaza gönder. Geçersiz token tespit edilirse true döner (silmek için).</summary>
    public async Task<FcmSendResult> SendAsync(string fcmToken, string title, string body, IDictionary<string, string>? data = null, CancellationToken ct = default)
    {
        if (!_initialized) return FcmSendResult.NotInitialized;

        var message = new Message
        {
            Token = fcmToken,
            Notification = new Notification { Title = title, Body = body },
            Data = data is null ? null : new Dictionary<string, string>(data)
        };

        try
        {
            var id = await FirebaseMessaging.DefaultInstance.SendAsync(message, ct);
            _logger.LogInformation("FCM gönderildi: {MessageId} → {TokenSuffix}", id, Suffix(fcmToken));
            return FcmSendResult.Success;
        }
        catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.Unregistered
                                                  || ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
        {
            _logger.LogWarning("FCM token geçersiz ({Code}): {TokenSuffix} → silinmesi gerekiyor", ex.MessagingErrorCode, Suffix(fcmToken));
            return FcmSendResult.InvalidToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FCM gönderim hatası: {TokenSuffix}", Suffix(fcmToken));
            return FcmSendResult.Failed;
        }
    }

    private static string Suffix(string token) => token.Length > 16 ? "..." + token[^8..] : token;
}

public enum FcmSendResult
{
    Success,
    InvalidToken,
    Failed,
    NotInitialized
}
