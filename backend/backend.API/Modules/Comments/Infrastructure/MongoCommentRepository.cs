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
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var carIdIndex = new CreateIndexModel<Comment>(
            Builders<Comment>.IndexKeys.Ascending(c => c.CarId));
        _mongoCollection.Indexes.CreateOne(carIdIndex);
    }

    public async Task CreateAsync(Comment comment)
    => await _mongoCollection.InsertOneAsync(comment);

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<Comment>.Filter.Eq(x => x.Id, id);
        await _mongoCollection.DeleteOneAsync(filter);
        return true;
    }

    public async Task<PagedResult<Comment>> GetByCarIdAsync(string carId, PaginationParameters pagination)
    {
        var filter = Builders<Comment>.Filter.Eq(c => c.CarId, carId);
        var total = await _mongoCollection.CountDocumentsAsync(filter);
        var data = await _mongoCollection.Find(filter)
            .SortByDescending(c => c.CreatedAt)
            .Skip(pagination.Offset)
            .Limit(pagination.Limit)
            .ToListAsync();

        return PagedResult<Comment>.Create(data, total, pagination.Limit, pagination.Offset);
    }

    public async Task<Comment> GetByCommentIdAsync(string id)
    {
        var filter = Builders<Comment>.Filter.Eq(x => x.Id, id);
        var comment = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
        return comment;

    }

    public async Task UpdateAsync(Comment comment)
    {
        var filter = Builders<Comment>.Filter.Eq(x => x.Id, comment.Id);
        await _mongoCollection.ReplaceOneAsync(filter, comment);
    }

    public async Task DeleteAllByUserIdAsync(string userId)
    {

        await _mongoCollection.DeleteManyAsync(c => c.UserId == userId);
    }

    public async Task DeleteAllByCarIdAsync(string carId)
    {

        await _mongoCollection.DeleteManyAsync(c => c.CarId == carId);
    }
}