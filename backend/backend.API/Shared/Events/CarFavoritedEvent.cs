namespace backend.API.Shared.Events;

public record CarFavoritedEvent(
    string IlanId,
    string KullaniciId,
    string ListeId,
    DateTime OlusturulmaTarihi);
