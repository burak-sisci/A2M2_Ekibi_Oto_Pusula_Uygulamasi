using MediatR;

namespace backend.API.Modules.Auth.Application;

public record DeleteUserCommand(string UserId) : IRequest<bool>;