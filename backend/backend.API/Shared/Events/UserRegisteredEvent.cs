namespace backend.API.Shared.Events;

public record UserRegisteredEvent(
    string KullaniciId,
    string Email,
    string Ad,
    DateTime OlusturulmaTarihi);
