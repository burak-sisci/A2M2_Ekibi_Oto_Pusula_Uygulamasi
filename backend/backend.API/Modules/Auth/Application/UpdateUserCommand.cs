using MediatR;

namespace backend.API.Modules.Auth.Application;

public record UpdateProfileCommand(
    string UserId, 
    string? Phone
) : IRequest<bool>;