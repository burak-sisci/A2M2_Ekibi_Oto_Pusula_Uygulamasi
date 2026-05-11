using MediatR;
using backend.API.Shared.Events;

namespace backend.API.Modules.Cars.Application;

public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, bool>
{
    private readonly ICarRepository _carRepository;
    private readonly ICarCacheService _cache;
    private readonly IMediator _mediator;

    public DeleteCarCommandHandler(ICarRepository carRepository, ICarCacheService cache, IMediator mediator)
    {
        _carRepository = carRepository;
        _cache         = cache;
        _mediator      = mediator;
    }

    public async Task<bool> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
    {
        var silindi = await _carRepository.DeleteAsync(request.CarId);

        if (silindi)
        {
            await _cache.InvalidateListCacheAsync();
            await _mediator.Publish(new CarDeletedEvent(request.CarId), cancellationToken);
        }

        return silindi;
    }
}
