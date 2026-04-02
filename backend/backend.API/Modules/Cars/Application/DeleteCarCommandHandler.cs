using MediatR;
using backend.API.Shared.Events;

namespace backend.API.Modules.Cars.Application;

public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, bool>
{
    private readonly ICarRepository _carRepository;
    private readonly IMediator _mediator;

    public DeleteCarCommandHandler(ICarRepository carRepository, IMediator mediator)
    {
        _carRepository = carRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
    {
        var isDeleted = await _carRepository.DeleteAsync(request.CarId);

        if (isDeleted)
        {
            await _mediator.Publish(new CarDeletedEvent(request.CarId), cancellationToken);
        }

        return isDeleted;
    }
}