using A2M2.API.DTOs;
using A2M2.API.Models;
using MongoDB.Driver;

namespace A2M2.API.Services;

/// <summary>
/// Kullanıcı profil servisi — Görüntüleme, Güncelleme, Silme (Modül A)
/// </summary>
public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    /// <summary>
    /// Kullanıcı profili getir — GET /users/{userId}
    /// </summary>
    public async Task<UserProfileResponse?> GetProfileAsync(string userId)
    {
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        if (user == null) return null;

        return new UserProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Gender = user.Gender,
            BirthDate = user.BirthDate,
            CreatedAt = user.CreatedAt,
        };
    }

    /// <summary>
    /// Profil güncelle — PUT /users/{userId}
    /// </summary>
    public async Task<UserProfileResponse?> UpdateProfileAsync(string userId, UpdateUserRequest request)
    {
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        if (user == null) return null;

        var updates = new List<UpdateDefinition<User>>();
        var builder = Builders<User>.Update;

        if (request.Name != null) updates.Add(builder.Set(u => u.Name, request.Name));
        if (request.Phone != null) updates.Add(builder.Set(u => u.Phone, request.Phone));
        if (request.Gender != null) updates.Add(builder.Set(u => u.Gender, request.Gender));
        if (request.BirthDate.HasValue) updates.Add(builder.Set(u => u.BirthDate, request.BirthDate));
        if (request.Password != null)
            updates.Add(builder.Set(u => u.PasswordHash, BCrypt.Net.BCrypt.HashPassword(request.Password)));
        updates.Add(builder.Set(u => u.UpdatedAt, DateTime.UtcNow));

        if (updates.Count > 0)
        {
            var combined = builder.Combine(updates);
            await _users.UpdateOneAsync(u => u.Id == userId, combined);
        }

        return await GetProfileAsync(userId);
    }

    /// <summary>
    /// Hesap sil — DELETE /users/{userId}
    /// </summary>
    public async Task<bool> DeleteAccountAsync(string userId)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == userId);
        return result.DeletedCount > 0;
    }
}
