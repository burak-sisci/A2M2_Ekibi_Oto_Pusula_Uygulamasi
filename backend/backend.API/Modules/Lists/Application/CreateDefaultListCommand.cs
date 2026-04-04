using backend.API.Modules.Lists.Application;
using backend.API.Modules.Lists.Domain;

namespace backend.API.Modules.Lists.Application;

public class CreateDefaultListCommand
{
    private readonly IListRepository _listRepository;

    public CreateDefaultListCommand(IListRepository listRepository)
    {
        _listRepository = listRepository;
    }

    public async Task ExecuteAsync(string userId)
    {
        await _listRepository.CreateDefaultListAsync(userId);
    }
}

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
