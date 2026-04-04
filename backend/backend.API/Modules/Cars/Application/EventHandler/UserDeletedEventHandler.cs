using MediatR;
using backend.API.Shared.Events;

namespace backend.API.Modules.Cars.Application.EventHandlers;

public class UserDeletedEventHandler : INotificationHandler<UserDeletedEvent>
{
    private readonly ICarRepository _carRepository;

    public UserDeletedEventHandler(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _carRepository.DeleteAllByOwnerIdAsync(notification.UserId);
    }
}