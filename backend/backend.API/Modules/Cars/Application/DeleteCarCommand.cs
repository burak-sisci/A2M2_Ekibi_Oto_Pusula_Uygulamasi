using MediatR;

namespace backend.API.Modules.Cars.Application;

public record DeleteCarCommand(string CarId) : IRequest<bool>;