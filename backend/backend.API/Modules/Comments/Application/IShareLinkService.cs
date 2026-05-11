namespace backend.API.Modules.Comments.Application;

public interface IShareLinkService
{
    Task<ShareLinkSonuc> GetOrCreateAsync(string carId, string baseUrl);
}

public record ShareLinkSonuc(string KisaUrl, string OrijinalUrl, DateTime? GecerlilikSonu);
