using backend.API.Modules.Auth.Domain;
using backend.API.Modules.Cars.Domain;
using backend.API.Modules.Comments.Domain;
using backend.API.Modules.Lists.Domain;
using MongoDB.Driver;

namespace backend.API.Shared.Database;

/// <summary>
/// Uygulama başlarken MongoDB indexlerini tek seferlik oluşturur.
/// Her istek constructor'ında senkron index oluşturma yerine burada async yapılır.
/// </summary>
public class MongoIndexInitializer : IHostedService
{
    private readonly MongoDbContext _context;
    private readonly ILogger<MongoIndexInitializer> _logger;

    public MongoIndexInitializer(MongoDbContext context, ILogger<MongoIndexInitializer> logger)
    {
        _context = context;
        _logger  = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MongoDB index oluşturma başladı...");
        try
        {
            await CreateUserIndexesAsync(cancellationToken);
            await CreateCarIndexesAsync(cancellationToken);
            await CreateCommentIndexesAsync(cancellationToken);
            await CreateListIndexesAsync(cancellationToken);
            _logger.LogInformation("MongoDB indexleri başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            // Index oluşturma başarısız olsa bile uygulama ayakta kalır.
            // Bağlantı sorunu varsa DB işlemleri yine hata verir, ama en azından
            // her istekte yeniden denenmiş olmaz.
            _logger.LogError(ex, "MongoDB index oluşturulurken hata: {Message}", ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // ── Users ──────────────────────────────────────────────────────────────────
    private async Task CreateUserIndexesAsync(CancellationToken ct)
    {
        var col = _context.GetCollection<User>("users");

        // Eski index adlarını temizle (şema değişikliklerinden kalma)
        foreach (var stale in new[] { "email_1", "phone_1", "telefon_1" })
        {
            try { await col.Indexes.DropOneAsync(stale, ct); } catch { /* yok ise sorun değil */ }
        }

        var emailIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true, Sparse = true, Name = "email_unique" });

        var phoneIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Phone),
            new CreateIndexOptions { Unique = true, Sparse = true, Name = "phone_unique" });

        await col.Indexes.CreateManyAsync([emailIndex, phoneIndex], cancellationToken: ct);
    }

    // ── Cars ───────────────────────────────────────────────────────────────────
    private async Task CreateCarIndexesAsync(CancellationToken ct)
    {
        var col = _context.GetCollection<Car>("cars");

        var compoundIndex = new CreateIndexModel<Car>(
            Builders<Car>.IndexKeys
                .Ascending(c => c.Marka)
                .Ascending(c => c.Seri)
                .Ascending(c => c.Model)
                .Ascending(c => c.Yil)
                .Ascending(c => c.Fiyat)
                .Ascending(c => c.Kilometre)
                .Ascending(c => c.VitesTipi)
                .Ascending(c => c.YakitTipi)
                .Ascending(c => c.KasaTipi)
                .Ascending(c => c.Cekis)
                .Ascending(c => c.Konum),
            new CreateIndexOptions { Name = "cars_search_compound" });

        await col.Indexes.CreateOneAsync(compoundIndex, cancellationToken: ct);
    }

    // ── Comments ───────────────────────────────────────────────────────────────
    private async Task CreateCommentIndexesAsync(CancellationToken ct)
    {
        var col = _context.GetCollection<Comment>("comments");

        var carIdIndex = new CreateIndexModel<Comment>(
            Builders<Comment>.IndexKeys.Ascending(c => c.CarId),
            new CreateIndexOptions { Name = "comment_carId" });

        var userIdIndex = new CreateIndexModel<Comment>(
            Builders<Comment>.IndexKeys.Ascending(c => c.UserId),
            new CreateIndexOptions { Name = "comment_userId" });

        await col.Indexes.CreateManyAsync([carIdIndex, userIdIndex], cancellationToken: ct);
    }

    // ── Lists ──────────────────────────────────────────────────────────────────
    private async Task CreateListIndexesAsync(CancellationToken ct)
    {
        var col = _context.GetCollection<UserList>("lists");

        var userIdIndex = new CreateIndexModel<UserList>(
            Builders<UserList>.IndexKeys.Ascending(l => l.UserId),
            new CreateIndexOptions { Name = "list_userId" });

        await col.Indexes.CreateOneAsync(userIdIndex, cancellationToken: ct);
    }
}
