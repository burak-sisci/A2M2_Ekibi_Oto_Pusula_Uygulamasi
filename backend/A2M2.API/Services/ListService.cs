using A2M2.API.DTOs;
using A2M2.API.Models;
using MongoDB.Driver;

namespace A2M2.API.Services;

/// <summary>
/// Liste servisi — Oluşturma, Silme, İlan Ekleme/Çıkarma (Modül C)
/// </summary>
public class ListService
{
    private readonly IMongoCollection<UserList> _lists;
    private readonly IMongoCollection<Listing> _listings;

    public ListService(IMongoDatabase database)
    {
        _lists = database.GetCollection<UserList>("lists");
        _listings = database.GetCollection<Listing>("listings");
    }

    /// <summary>
    /// Varsayılan "Favoriler" listesi oluştur (kayıt sırasında çağrılır)
    /// </summary>
    public async Task CreateDefaultListAsync(string userId)
    {
        var defaultList = new UserList
        {
            UserId = userId,
            Name = "Favoriler",
            IsDefault = true,
        };
        await _lists.InsertOneAsync(defaultList);
    }

    /// <summary>
    /// Yeni liste oluştur — POST /lists
    /// </summary>
    public async Task<UserList> CreateAsync(string userId, CreateListRequest request)
    {
        var list = new UserList
        {
            UserId = userId,
            Name = request.Name,
            IsDefault = false,
        };
        await _lists.InsertOneAsync(list);
        return list;
    }

    /// <summary>
    /// Listeye ilan ekle — POST /lists/{listId}/cars
    /// </summary>
    public async Task<UserList?> AddCarAsync(string listId, string userId, string carId)
    {
        var list = await _lists.Find(l => l.Id == listId && l.UserId == userId).FirstOrDefaultAsync();
        if (list == null) return null;

        if (list.Cars.Contains(carId))
            throw new InvalidOperationException("Bu araç zaten listede");

        var update = Builders<UserList>.Update
            .Push(l => l.Cars, carId)
            .Set(l => l.UpdatedAt, DateTime.UtcNow);
        await _lists.UpdateOneAsync(l => l.Id == listId, update);

        return await _lists.Find(l => l.Id == listId).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Kullanıcının tüm listelerini getir — GET /users/{userId}/lists
    /// </summary>
    public async Task<List<object>> GetUserListsAsync(string userId)
    {
        var lists = await _lists.Find(l => l.UserId == userId).ToListAsync();
        return lists.Select(l => (object)new
        {
            l.Id,
            l.Name,
            l.IsDefault,
            CarCount = l.Cars.Count,
            l.CreatedAt,
            l.UpdatedAt,
        }).ToList();
    }

    /// <summary>
    /// Liste içeriğini getir (araç detaylarıyla) — GET /lists/{listId}
    /// </summary>
    public async Task<object?> GetListDetailAsync(string listId, string userId)
    {
        var list = await _lists.Find(l => l.Id == listId && l.UserId == userId).FirstOrDefaultAsync();
        if (list == null) return null;

        var cars = new List<Listing>();
        foreach (var carId in list.Cars)
        {
            var car = await _listings.Find(c => c.Id == carId).FirstOrDefaultAsync();
            if (car != null) cars.Add(car);
        }

        return new
        {
            list.Id,
            list.Name,
            list.IsDefault,
            Cars = cars,
            list.CreatedAt,
            list.UpdatedAt,
        };
    }

    /// <summary>
    /// Liste adını güncelle — PUT /lists/{listId} (Favoriler hariç)
    /// </summary>
    public async Task<UserList?> UpdateNameAsync(string listId, string userId, UpdateListRequest request)
    {
        var list = await _lists.Find(l => l.Id == listId && l.UserId == userId).FirstOrDefaultAsync();
        if (list == null) return null;
        if (list.IsDefault) throw new InvalidOperationException("Varsayılan listenin adı değiştirilemez");

        var update = Builders<UserList>.Update
            .Set(l => l.Name, request.Name)
            .Set(l => l.UpdatedAt, DateTime.UtcNow);
        await _lists.UpdateOneAsync(l => l.Id == listId, update);

        return await _lists.Find(l => l.Id == listId).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Listeden ilan çıkar — DELETE /lists/{listId}/cars/{carId}
    /// </summary>
    public async Task<bool> RemoveCarAsync(string listId, string userId, string carId)
    {
        var list = await _lists.Find(l => l.Id == listId && l.UserId == userId).FirstOrDefaultAsync();
        if (list == null) return false;

        var update = Builders<UserList>.Update
            .Pull(l => l.Cars, carId)
            .Set(l => l.UpdatedAt, DateTime.UtcNow);
        var result = await _lists.UpdateOneAsync(l => l.Id == listId, update);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Listeyi tamamen sil — DELETE /lists/{listId}
    /// </summary>
    public async Task<bool> DeleteAsync(string listId, string userId)
    {
        var list = await _lists.Find(l => l.Id == listId && l.UserId == userId).FirstOrDefaultAsync();
        if (list == null) return false;
        if (list.IsDefault) throw new InvalidOperationException("Varsayılan liste silinemez");

        var result = await _lists.DeleteOneAsync(l => l.Id == listId);
        return result.DeletedCount > 0;
    }
}
