using A2M2.API.Models;
using MongoDB.Driver;

namespace A2M2.API.Services;

/// <summary>
/// Favori servisi — Ekleme, Silme, Listeleme
/// </summary>
public class FavoriteService
{
    private readonly IMongoCollection<Favorite> _favorites;
    private readonly IMongoCollection<Listing> _listings;

    public FavoriteService(IMongoDatabase database)
    {
        _favorites = database.GetCollection<Favorite>("favorites");
        _listings = database.GetCollection<Listing>("listings");

        // Unique compound index: userId + listingId
        var indexKeys = Builders<Favorite>.IndexKeys
            .Ascending(f => f.UserId)
            .Ascending(f => f.ListingId);
        var indexOptions = new CreateIndexOptions { Unique = true };
        _favorites.Indexes.CreateOne(new CreateIndexModel<Favorite>(indexKeys, indexOptions));
    }

    /// <summary>
    /// Kullanıcının tüm favorilerini getir (listing bilgileriyle birlikte)
    /// </summary>
    public async Task<List<object>> GetUserFavoritesAsync(string userId)
    {
        var favorites = await _favorites.Find(f => f.UserId == userId).ToListAsync();
        var result = new List<object>();

        foreach (var fav in favorites)
        {
            var listing = await _listings.Find(l => l.Id == fav.ListingId).FirstOrDefaultAsync();
            result.Add(new
            {
                fav.Id,
                fav.UserId,
                fav.ListingId,
                fav.CreatedAt,
                Listing = listing,
            });
        }

        return result;
    }

    /// <summary>
    /// Favorilere ekle
    /// </summary>
    public async Task<Favorite> AddAsync(string userId, string listingId)
    {
        // Duplicate kontrolü
        var existing = await _favorites.Find(f => f.UserId == userId && f.ListingId == listingId).FirstOrDefaultAsync();
        if (existing != null)
            throw new InvalidOperationException("Bu ilan zaten favorilerinizde");

        var favorite = new Favorite
        {
            UserId = userId,
            ListingId = listingId,
        };

        await _favorites.InsertOneAsync(favorite);
        return favorite;
    }

    /// <summary>
    /// Favorilerden kaldır
    /// </summary>
    public async Task<bool> RemoveAsync(string userId, string listingId)
    {
        var result = await _favorites.DeleteOneAsync(f => f.UserId == userId && f.ListingId == listingId);
        return result.DeletedCount > 0;
    }
}
