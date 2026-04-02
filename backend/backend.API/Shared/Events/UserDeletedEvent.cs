using MediatR;

namespace backend.API.Shared.Events;

public record UserDeletedEvent(string UserId) : INotification;