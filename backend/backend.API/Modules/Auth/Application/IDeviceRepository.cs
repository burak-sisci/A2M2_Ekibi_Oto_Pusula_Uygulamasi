using backend.API.Modules.Auth.Domain;

namespace backend.API.Modules.Auth.Application;

public interface IDeviceRepository
{
    /// <summary>Aynı (userId, fcmToken) varsa güncelle, yoksa ekle.</summary>
    Task UpsertAsync(Device device);

    /// <summary>Token ile sil — kullanıcı çıkış yaptığında veya cihaz reset'lendiğinde.</summary>
    Task DeleteByTokenAsync(string fcmToken);

    /// <summary>Kullanıcının tüm cihazlarını getir (notification gönderimi için).</summary>
    Task<List<Device>> GetByUserIdAsync(string userId);
}
