using backend.API.Modules.Comments.Domain;
using backend.API.Modules.Comments.Application;
using backend.API.Shared.Database;
using backend.API.Shared.Paginition;
using MongoDB.Driver;

namespace backend.API.Modules.Comments.Infrastructure;

public class MongoCommentRepository : ICommentRepository
{
    private readonly IMongoCollection<Comment> _mongoCollection;

    public MongoCommentRepository(MongoDbContext mongoDbContext)
    {
        _mongoCollection = mongoDbContext.GetCollection<Comment>("comments");
        // Indexler artık MongoIndexInitializer (IHostedService) tarafından startup'ta oluşturulur.
    }

    public async Task CreateAsync(Comment yorum)
        => await _mongoCollection.InsertOneAsync(yorum);

    public async Task<bool> DeleteAsync(string id)
    {
        var sonuc = await _mongoCollection.DeleteOneAsync(x => x.Id == id);
        return sonuc.DeletedCount > 0;
    }

    public async Task<PagedResult<Comment>> GetByCarIdAsync(string carId, PaginationParameters sayfalama)
    {
        var filtre = Builders<Comment>.Filter.Eq(c => c.CarId, carId);
        var toplam = await _mongoCollection.CountDocumentsAsync(filtre);
        var veri   = await _mongoCollection.Find(filtre)
            .SortByDescending(c => c.CreatedAt)
            .Skip(sayfalama.Offset)
            .Limit(sayfalama.Limit)
            .ToListAsync();

        return PagedResult<Comment>.Create(veri, toplam, sayfalama.Limit, sayfalama.Offset);
    }

    public async Task<PagedResult<Comment>> GetByUserIdAsync(string userId, PaginationParameters sayfalama)
    {
        var filtre = Builders<Comment>.Filter.Eq(c => c.UserId, userId);
        var toplam = await _mongoCollection.CountDocumentsAsync(filtre);
        var veri   = await _mongoCollection.Find(filtre)
            .SortByDescending(c => c.CreatedAt)
            .Skip(sayfalama.Offset)
            .Limit(sayfalama.Limit)
            .ToListAsync();

        return PagedResult<Comment>.Create(veri, toplam, sayfalama.Limit, sayfalama.Offset);
    }

    public async Task<Comment?> GetByCommentIdAsync(string id)
    {
        var filtre = Builders<Comment>.Filter.Eq(x => x.Id, id);
        return await _mongoCollection.Find(filtre).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Comment yorum)
    {
        var filtre = Builders<Comment>.Filter.Eq(x => x.Id, yorum.Id);
        await _mongoCollection.ReplaceOneAsync(filtre, yorum);
    }

    public async Task DeleteAllByUserIdAsync(string userId)
        => await _mongoCollection.DeleteManyAsync(c => c.UserId == userId);

    public async Task DeleteAllByCarIdAsync(string carId)
        => await _mongoCollection.DeleteManyAsync(c => c.CarId == carId);

    public async Task<bool> LikeAsync(string commentId, string userId)
    {
        var guncelleme = Builders<Comment>.Update
            .AddToSet(c => c.LikedByUsers, userId)
            .Inc(c => c.LikeCount, 1)
            .Set(c => c.UpdatedAt, DateTime.UtcNow);

        var sonuc = await _mongoCollection.UpdateOneAsync(
            c => c.Id == commentId && !c.LikedByUsers.Contains(userId),
            guncelleme);

        return sonuc.ModifiedCount > 0;
    }

    public async Task<bool> UnlikeAsync(string commentId, string userId)
    {
        var guncelleme = Builders<Comment>.Update
            .Pull(c => c.LikedByUsers, userId)
            .Inc(c => c.LikeCount, -1)
            .Set(c => c.UpdatedAt, DateTime.UtcNow);

        var sonuc = await _mongoCollection.UpdateOneAsync(
            c => c.Id == commentId && c.LikedByUsers.Contains(userId),
            guncelleme);

        return sonuc.ModifiedCount > 0;
    }
}
