using MediatR;
using backend.API.Shared.Events;

namespace backend.API.Modules.Comments.Application.EventHandlers;

public class CarDeletedEventHandler : INotificationHandler<CarDeletedEvent>
{
    private readonly ICommentRepository _commentRepository;

    public CarDeletedEventHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task Handle(CarDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _commentRepository.DeleteAllByCarIdAsync(notification.CarId);
    }
}