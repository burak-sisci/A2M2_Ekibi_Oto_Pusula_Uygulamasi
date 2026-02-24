namespace A2M2.API.Configuration;

/// <summary>
/// MongoDB bağlantı ayarları — appsettings.json'dan okunur
/// </summary>
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}
