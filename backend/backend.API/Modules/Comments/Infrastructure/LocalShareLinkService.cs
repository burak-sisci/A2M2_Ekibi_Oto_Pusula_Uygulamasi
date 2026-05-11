using backend.API.Modules.Comments.Application;

namespace backend.API.Modules.Comments.Infrastructure;

/// <summary>Redis mevcut değilken kullanılan fallback — paylaşım linkleri bellekte tutulmaz.</summary>
public class LocalShareLinkService : IShareLinkService
{
    public Task<ShareLinkSonuc> GetOrCreateAsync(string carId, string baseUrl)
    {
        var kod        = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(6))
                            .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var kisaUrl    = $"{baseUrl}/s/{kod}";
        var orijinalUrl = $"{baseUrl}/cars/{carId}";
        var gecerlilik  = DateTime.UtcNow.AddDays(1);

        return Task.FromResult(new ShareLinkSonuc(kisaUrl, orijinalUrl, gecerlilik));
    }
}
