using MediatR;
using backend.API.Modules.Auth.Domain;

namespace backend.API.Modules.Auth.Application;

// Geriye dönük uyumluluk için eski record korunuyor
public record UpdateProfileCommand(
    string KullaniciId,
    string? Telefon) : IRequest<bool>;

// YAML'a uygun tam profil güncelleme komutu
public record UpdateUserCommand(
    string HedefKullaniciId,
    string IstekciKullaniciId,
    string? Ad,
    string? Telefon,
    string? YeniSifre) : IRequest<bool>;
