using backend.API.Modules.Lists.Domain;
using backend.API.Shared.Events;
using backend.API.Shared.Messaging;

namespace backend.API.Modules.Lists.Application;

public class CreateListCommand
{
    private readonly IListRepository _listRepository;
    private readonly IListCacheService _cache;

    public CreateListCommand(IListRepository listRepository, IListCacheService cache)
    {
        _listRepository = listRepository;
        _cache          = cache;
    }

    public async Task<UserList> ExecuteAsync(string userId, string listeAdi)
    {
        var liste = new UserList
        {
            UserId    = userId,
            ListName  = listeAdi,
            IsDefault = false
        };

        await _listRepository.CreateAsync(liste);
        try { await _cache.InvalidateUserListsCacheAsync(userId); } catch { /* Redis opsiyonel */ }

        return liste;
    }
}

public class AddCarToListCommand
{
    private readonly IListRepository _listRepository;
    private readonly IListCacheService _cache;
    private readonly IRabbitMqPublisher _publisher;

    public AddCarToListCommand(
        IListRepository listRepository,
        IListCacheService cache,
        IRabbitMqPublisher publisher)
    {
        _listRepository = listRepository;
        _cache          = cache;
        _publisher      = publisher;
    }

    public async Task<bool> ExecuteAsync(string listId, string carId, string userId)
    {
        var liste = await _listRepository.GetByIdAsync(listId);
        if (liste is null || liste.UserId != userId)
            throw new UnauthorizedAccessException("Bu listeye ekleme yetkiniz yok.");

        if (liste.Items.Contains(carId))
            throw new InvalidOperationException("İlan zaten listede mevcut.");

        var basarili = await _listRepository.AddItemAsync(listId, carId);

        if (basarili)
        {
            try { await _cache.InvalidateUserListsCacheAsync(userId); } catch { /* Redis opsiyonel */ }

            // RabbitMQ: Mehmet Öz — listeye araç ekleme event'i
            await _publisher.PublishAsync(
                new CarFavoritedEvent(carId, userId, listId, DateTime.UtcNow),
                RabbitMqQueues.IlanFavorilendi);
        }

        return basarili;
    }
}

public class RemoveCarFromListCommand
{
    private readonly IListRepository _listRepository;
    private readonly IListCacheService _cache;

    public RemoveCarFromListCommand(IListRepository listRepository, IListCacheService cache)
    {
        _listRepository = listRepository;
        _cache          = cache;
    }

    public async Task<bool> ExecuteAsync(string listId, string carId, string userId)
    {
        var liste = await _listRepository.GetByIdAsync(listId);
        if (liste is null || liste.UserId != userId)
            throw new UnauthorizedAccessException("Bu listeden çıkarma yetkiniz yok.");

        var basarili = await _listRepository.RemoveItemAsync(listId, carId);
        if (basarili)
            try { await _cache.InvalidateUserListsCacheAsync(userId); } catch { /* Redis opsiyonel */ }

        return basarili;
    }
}

public class UpdateListNameCommand
{
    private readonly IListRepository _listRepository;
    private readonly IListCacheService _cache;

    public UpdateListNameCommand(IListRepository listRepository, IListCacheService cache)
    {
        _listRepository = listRepository;
        _cache          = cache;
    }

    public async Task<bool> ExecuteAsync(string listId, string userId, string yeniAd)
    {
        var liste = await _listRepository.GetByIdAsync(listId);
        if (liste is null) return false;

        if (liste.UserId != userId)
            throw new UnauthorizedAccessException("Bu listeyi güncelleme yetkiniz yok.");

        if (liste.IsDefault)
            throw new InvalidOperationException("Varsayılan Favoriler listesinin adı değiştirilemez.");

        var basarili = await _listRepository.UpdateNameAsync(listId, userId, yeniAd);
        if (basarili)
            try { await _cache.InvalidateUserListsCacheAsync(userId); } catch { /* Redis opsiyonel */ }

        return basarili;
    }
}

public class DeleteListCommand
{
    private readonly IListRepository _listRepository;
    private readonly IListCacheService _cache;

    public DeleteListCommand(IListRepository listRepository, IListCacheService cache)
    {
        _listRepository = listRepository;
        _cache          = cache;
    }

    public async Task<bool> ExecuteAsync(string listId, string userId)
    {
        var liste = await _listRepository.GetByIdAsync(listId);
        if (liste is null) return false;

        if (liste.IsDefault)
            throw new InvalidOperationException("Varsayılan Favoriler listesi silinemez.");

        var basarili = await _listRepository.DeleteAsync(listId, userId);
        if (basarili)
            try { await _cache.InvalidateUserListsCacheAsync(userId); } catch { /* Redis opsiyonel */ }

        return basarili;
    }
}

// Geriye dönük uyumluluk için eski command
public class AddItemToListCommand
{
    private readonly IListRepository _listRepository;

    public AddItemToListCommand(IListRepository listRepository)
    {
        _listRepository = listRepository;
    }

    public async Task<bool> ExecuteAsync(string listId, string carId)
        => await _listRepository.AddItemAsync(listId, carId);
}

public class CreateDefaultListCommand
{
    private readonly IListRepository _listRepository;

    public CreateDefaultListCommand(IListRepository listRepository)
    {
        _listRepository = listRepository;
    }

    public async Task ExecuteAsync(string userId)
        => await _listRepository.CreateDefaultListAsync(userId);
}
