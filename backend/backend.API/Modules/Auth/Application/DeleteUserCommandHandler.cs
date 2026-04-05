using MediatR;
using backend.API.Shared.Events;

namespace backend.API.Modules.Auth.Application;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    public DeleteUserCommandHandler(IUserRepository userRepository, IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var isDeleted = await _userRepository.DeleteAsync(request.UserId);

        if (isDeleted)
        {
            await _mediator.Publish(new UserDeletedEvent(request.UserId), cancellationToken);
        }

        return isDeleted;
    }
}