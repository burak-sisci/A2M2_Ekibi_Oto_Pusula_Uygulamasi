using MediatR;
using backend.API.Shared.Events;

namespace backend.API.Modules.Comments.Application.EventHandlers;

public class UserDeletedEventHandler : INotificationHandler<UserDeletedEvent>
{
    private readonly ICommentRepository _commentRepository;

    public UserDeletedEventHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _commentRepository.DeleteAllByUserIdAsync(notification.UserId);
    }
}