using MediatR;

namespace backend.API.Shared.Events;

public record CarDeletedEvent(string CarId) : INotification;