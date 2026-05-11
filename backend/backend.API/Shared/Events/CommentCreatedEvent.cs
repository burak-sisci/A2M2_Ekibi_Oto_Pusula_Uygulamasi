namespace backend.API.Shared.Events;

public record CommentCreatedEvent(
    string YorumId,
    string IlanId,
    string YazarId,
    DateTime OlusturulmaTarihi);
