namespace backend.API.Shared.Events;

public record CarCreatedEvent(
    string IlanId,
    string SahibiId,
    string Marka,
    string Model,
    DateTime OlusturulmaTarihi);
