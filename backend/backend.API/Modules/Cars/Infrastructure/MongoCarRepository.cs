using backend.API.Modules.Cars.Application;
using backend.API.Modules.Cars.Domain;
using backend.API.Shared.Database;
using backend.API.Shared.Paginition;
using MongoDB.Driver;

namespace backend.API.Modules.Cars.Infrastructure;

public class MongoCarRepository : ICarRepository
{
    private readonly IMongoCollection<Car> _collection;

    public MongoCarRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<Car>("cars");
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var compoundIndex = new CreateIndexModel<Car>(
            Builders<Car>.IndexKeys
                .Ascending(c => c.Marka)
                .Ascending(c => c.Seri)
                .Ascending(c => c.Model)
                .Ascending(c => c.Yil)
                .Ascending(c => c.Fiyat)
                .Ascending(c => c.Kilometre)
                .Ascending("fuelType")          // YakitTipi
                .Ascending("transmissionType")  // VitesTipi
                .Ascending("bodyType")          // KasaTipi
                .Ascending("driveType")         // Cekis
                .Ascending(c => c.Konum));

        _collection.Indexes.CreateOne(compoundIndex);
    }

    public async Task<PagedResult<Car>> GetAllAsync(CarsFilter filter, PaginationParameters pagination)
    {
        var builder = Builders<Car>.Filter;
        var filterDef = builder.Empty;

        // Metin filtreleri
        if (!string.IsNullOrWhiteSpace(filter.Marka))
            filterDef &= builder.Regex(c => c.Marka, new MongoDB.Bson.BsonRegularExpression(filter.Marka, "i"));

        if (!string.IsNullOrWhiteSpace(filter.Seri))
            filterDef &= builder.Regex(c => c.Seri, new MongoDB.Bson.BsonRegularExpression(filter.Seri, "i"));

        if (!string.IsNullOrWhiteSpace(filter.Model))
            filterDef &= builder.Regex(c => c.Model, new MongoDB.Bson.BsonRegularExpression(filter.Model, "i"));

        if (!string.IsNullOrWhiteSpace(filter.Konum))
            filterDef &= builder.Regex(c => c.Konum, new MongoDB.Bson.BsonRegularExpression(filter.Konum, "i"));

        if (!string.IsNullOrWhiteSpace(filter.Renk))
            filterDef &= builder.Regex(c => c.Renk, new MongoDB.Bson.BsonRegularExpression(filter.Renk, "i"));

        // Fiyat filtresi
        if (filter.MinFiyat.HasValue)
            filterDef &= builder.Gte(c => c.Fiyat, filter.MinFiyat.Value);

        if (filter.MaxFiyat.HasValue)
            filterDef &= builder.Lte(c => c.Fiyat, filter.MaxFiyat.Value);

        // Kilometre filtresi
        if (filter.MinKilometre.HasValue)
            filterDef &= builder.Gte(c => c.Kilometre, filter.MinKilometre.Value);

        if (filter.MaxKilometre.HasValue)
            filterDef &= builder.Lte(c => c.Kilometre, filter.MaxKilometre.Value);

        // Yıl filtresi
        if (filter.MinYil.HasValue)
            filterDef &= builder.Gte(c => c.Yil, filter.MinYil.Value);

        if (filter.MaxYil.HasValue)
            filterDef &= builder.Lte(c => c.Yil, filter.MaxYil.Value);

        // Motor gücü filtresi
        if (filter.MinMotorGucu.HasValue)
            filterDef &= builder.Gte(c => c.MotorGucu, filter.MinMotorGucu.Value);

        if (filter.MaxMotorGucu.HasValue)
            filterDef &= builder.Lte(c => c.MotorGucu, filter.MaxMotorGucu.Value);

        // Enum filtreleri
        if (filter.VitesTipi.HasValue)
            filterDef &= builder.Eq(c => c.VitesTipi, filter.VitesTipi.Value);

        if (filter.YakitTipi.HasValue)
            filterDef &= builder.Eq(c => c.YakitTipi, filter.YakitTipi.Value);

        if (filter.KasaTipi.HasValue)
            filterDef &= builder.Eq(c => c.KasaTipi, filter.KasaTipi.Value);

       // if (filter.Cekis.HasValue)
           // filterDef &= builder.Eq(c=> c.Cekis,filter.Cekis.Value);

        if (filter.AracDurumu.HasValue)
            filterDef &= builder.Eq(c => c.AracDurumu, filter.AracDurumu.Value);

        if (filter.Kimden.HasValue)
            filterDef &= builder.Eq(c => c.Kimden, filter.Kimden.Value);

        // Boolean filtreleri
        if (filter.AgirHasarKaydi.HasValue)
            filterDef &= builder.Eq(c => c.AgirHasarKaydi, filter.AgirHasarKaydi.Value);

        if (filter.TakasaUygun.HasValue)
            filterDef &= builder.Eq(c => c.TakasaUygun, filter.TakasaUygun.Value);

        var total = await _collection.CountDocumentsAsync(filterDef);
        var data = await _collection.Find(filterDef)
            .Skip(pagination.Offset)
            .Limit(pagination.Limit)
            .ToListAsync();

        return PagedResult<Car>.Create(data, total, pagination.Limit, pagination.Offset);
    }

    public async Task<Car?> GetByIdAsync(string id)
        => await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Car car)
        => await _collection.InsertOneAsync(car);

    public async Task<bool> UpdateAsync(string id, Car car)
    {
        var result = await _collection.ReplaceOneAsync(c => c.Id == id, car);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task DeleteAllByOwnerIdAsync(string ownerId)
        => await _collection.DeleteManyAsync(c => c.IlanSahibi == ownerId);
}