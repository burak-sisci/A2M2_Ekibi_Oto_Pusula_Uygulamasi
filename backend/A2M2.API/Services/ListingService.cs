using A2M2.API.DTOs;
using A2M2.API.Models;
using MongoDB.Driver;

namespace A2M2.API.Services;

/// <summary>
/// İlan servisi — CRUD + filtreleme + sıralama (Modül B)
/// </summary>
public class ListingService
{
    private readonly IMongoCollection<Listing> _listings;

    public ListingService(IMongoDatabase database)
    {
        _listings = database.GetCollection<Listing>("listings");
    }

    /// <summary>
    /// Tüm ilanları getir (filtreleme + sıralama)
    /// </summary>
    public async Task<List<Listing>> GetAllAsync(ListingFilterParams? filters = null)
    {
        var builder = Builders<Listing>.Filter;
        var filter = builder.Empty;

        if (filters != null)
        {
            if (!string.IsNullOrEmpty(filters.Brand))
                filter &= builder.Regex(l => l.Brand, new MongoDB.Bson.BsonRegularExpression(filters.Brand, "i"));

            if (!string.IsNullOrEmpty(filters.FuelType))
                filter &= builder.Eq(l => l.FuelType, filters.FuelType);

            if (!string.IsNullOrEmpty(filters.GearType))
                filter &= builder.Eq(l => l.GearType, filters.GearType);

            if (filters.MinPrice.HasValue)
                filter &= builder.Gte(l => l.Price, filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                filter &= builder.Lte(l => l.Price, filters.MaxPrice.Value);

            if (!string.IsNullOrEmpty(filters.Location))
                filter &= builder.Regex("location.city", new MongoDB.Bson.BsonRegularExpression(filters.Location, "i"));
        }

        var sort = Builders<Listing>.Sort.Descending(l => l.CreatedAt);
        if (filters?.SortBy != null)
        {
            var isAsc = filters.SortOrder?.ToLower() == "asc";
            sort = filters.SortBy.ToLower() switch
            {
                "price" => isAsc ? Builders<Listing>.Sort.Ascending(l => l.Price)
                                 : Builders<Listing>.Sort.Descending(l => l.Price),
                "year"  => isAsc ? Builders<Listing>.Sort.Ascending(l => l.Year)
                                 : Builders<Listing>.Sort.Descending(l => l.Year),
                "km"    => isAsc ? Builders<Listing>.Sort.Ascending(l => l.Km)
                                 : Builders<Listing>.Sort.Descending(l => l.Km),
                _       => Builders<Listing>.Sort.Descending(l => l.CreatedAt),
            };
        }

        return await _listings.Find(filter).Sort(sort).ToListAsync();
    }

    /// <summary>
    /// ID ile ilan getir
    /// </summary>
    public async Task<Listing?> GetByIdAsync(string id)
    {
        return await _listings.Find(l => l.Id == id).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Yeni ilan oluştur
    /// </summary>
    public async Task<Listing> CreateAsync(CreateListingRequest request, string userId)
    {
        var listing = new Listing
        {
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            Km = request.Km,
            FuelType = request.FuelType,
            GearType = request.GearType,
            Price = request.Price,
            Description = request.Description,
            Images = request.Images ?? new List<string>(),
            Location = request.Location,
            DamageInfo = request.DamageInfo ?? new List<string>(),
            UserId = userId,
        };

        await _listings.InsertOneAsync(listing);
        return listing;
    }

    /// <summary>
    /// İlan güncelle (sadece sahibi)
    /// </summary>
    public async Task<Listing?> UpdateAsync(string id, UpdateListingRequest request, string userId)
    {
        var listing = await _listings.Find(l => l.Id == id).FirstOrDefaultAsync();
        if (listing == null) return null;
        if (listing.UserId != userId) throw new UnauthorizedAccessException("Bu ilanı düzenleme yetkiniz yok");

        var updates = new List<UpdateDefinition<Listing>>();
        var b = Builders<Listing>.Update;

        if (request.Brand != null) updates.Add(b.Set(l => l.Brand, request.Brand));
        if (request.Model != null) updates.Add(b.Set(l => l.Model, request.Model));
        if (request.Year.HasValue) updates.Add(b.Set(l => l.Year, request.Year.Value));
        if (request.Km.HasValue) updates.Add(b.Set(l => l.Km, request.Km.Value));
        if (request.FuelType != null) updates.Add(b.Set(l => l.FuelType, request.FuelType));
        if (request.GearType != null) updates.Add(b.Set(l => l.GearType, request.GearType));
        if (request.Price.HasValue) updates.Add(b.Set(l => l.Price, request.Price.Value));
        if (request.Description != null) updates.Add(b.Set(l => l.Description, request.Description));
        if (request.Images != null) updates.Add(b.Set(l => l.Images, request.Images));
        if (request.Location != null) updates.Add(b.Set(l => l.Location, request.Location));
        if (request.DamageInfo != null) updates.Add(b.Set(l => l.DamageInfo, request.DamageInfo));
        updates.Add(b.Set(l => l.UpdatedAt, DateTime.UtcNow));

        if (updates.Count > 0)
        {
            var combined = b.Combine(updates);
            await _listings.UpdateOneAsync(l => l.Id == id, combined);
        }

        return await GetByIdAsync(id);
    }

    /// <summary>
    /// İlan sil (sadece sahibi)
    /// </summary>
    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var listing = await _listings.Find(l => l.Id == id).FirstOrDefaultAsync();
        if (listing == null) return false;
        if (listing.UserId != userId) throw new UnauthorizedAccessException("Bu ilanı silme yetkiniz yok");

        var result = await _listings.DeleteOneAsync(l => l.Id == id);
        return result.DeletedCount > 0;
    }
}
