using A2M2.API.DTOs;
using A2M2.API.Models;
using MongoDB.Driver;

namespace A2M2.API.Services;

/// <summary>
/// Yorum servisi — Ekleme, Listeleme, Güncelleme, Silme (Modül D)
/// </summary>
public class CommentService
{
    private readonly IMongoCollection<Comment> _comments;

    public CommentService(IMongoDatabase database)
    {
        _comments = database.GetCollection<Comment>("comments");
    }

    /// <summary>
    /// Yorum ekle — POST /cars/{carId}/comments
    /// </summary>
    public async Task<Comment> CreateAsync(string carId, string userId, CreateCommentRequest request)
    {
        var comment = new Comment
        {
            CarId = carId,
            UserId = userId,
            Text = request.Text,
        };
        await _comments.InsertOneAsync(comment);
        return comment;
    }

    /// <summary>
    /// İlan yorumlarını listele — GET /cars/{carId}/comments
    /// </summary>
    public async Task<List<Comment>> GetByCarAsync(string carId)
    {
        return await _comments.Find(c => c.CarId == carId)
            .Sort(Builders<Comment>.Sort.Ascending(c => c.CreatedAt))
            .ToListAsync();
    }

    /// <summary>
    /// Yorum güncelle — PUT /comments/{commentId} (sadece sahibi)
    /// </summary>
    public async Task<Comment?> UpdateAsync(string commentId, string userId, UpdateCommentRequest request)
    {
        var comment = await _comments.Find(c => c.Id == commentId).FirstOrDefaultAsync();
        if (comment == null) return null;
        if (comment.UserId != userId) throw new UnauthorizedAccessException("Bu yorumu düzenleme yetkiniz yok");

        var update = Builders<Comment>.Update
            .Set(c => c.Text, request.Text)
            .Set(c => c.UpdatedAt, DateTime.UtcNow);
        await _comments.UpdateOneAsync(c => c.Id == commentId, update);

        return await _comments.Find(c => c.Id == commentId).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Yorum sil — DELETE /comments/{commentId} (sadece sahibi)
    /// </summary>
    public async Task<bool> DeleteAsync(string commentId, string userId)
    {
        var comment = await _comments.Find(c => c.Id == commentId).FirstOrDefaultAsync();
        if (comment == null) return false;
        if (comment.UserId != userId) throw new UnauthorizedAccessException("Bu yorumu silme yetkiniz yok");

        var result = await _comments.DeleteOneAsync(c => c.Id == commentId);
        return result.DeletedCount > 0;
    }

    /// <summary>
    /// Kullanıcının tüm yorumlarını getir — GET /users/{userId}/comments
    /// </summary>
    public async Task<List<Comment>> GetByUserAsync(string userId)
    {
        return await _comments.Find(c => c.UserId == userId)
            .Sort(Builders<Comment>.Sort.Descending(c => c.CreatedAt))
            .ToListAsync();
    }
}
