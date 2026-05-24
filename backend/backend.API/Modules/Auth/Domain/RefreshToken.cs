namespace backend.API.Modules.Auth.Domain;

/// <summary>
/// Redis'te `refresh:{userId}:{tokenId}` key'iyle saklanır. Body'de bu sınıfın JSON hâli durur.
/// Token'ın kendisi key'e gömülmez; gelen istekteki token, store'daki TokenHash ile eşleştirilir.
/// </summary>
public class RefreshToken
{
    public string TokenId    { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId     { get; set; } = default!;
    public string TokenHash  { get; set; } = default!;   // SHA-256 of raw token
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string DeviceInfo { get; set; } = "";
}
