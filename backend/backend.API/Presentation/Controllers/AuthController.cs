using backend.API.Modules.Auth.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;

namespace backend.API.Presentation.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserCommand _registerCommand;
    private readonly LoginUserCommand _loginCommand;
    private readonly LogoutUserCommand _logoutCommand;
    private readonly IMediator _mediator;

    public AuthController(
        RegisterUserCommand registerCommand,
        LoginUserCommand loginCommand,
        LogoutUserCommand logoutCommand,
        IMediator mediator)
    {
        _registerCommand = registerCommand;
        _loginCommand    = loginCommand;
        _logoutCommand   = logoutCommand;
        _mediator        = mediator;
    }

    /// <summary>POST /auth/register — Yeni kullanıcı kaydı</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest istek)
    {
        var sonuc = await _registerCommand.ExecuteAsync(istek);
        return StatusCode(201, sonuc);
    }

    /// <summary>POST /auth/login — Kullanıcı girişi (email veya telefon)</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest istek)
    {
        var sonuc = await _loginCommand.ExecuteAsync(istek);
        return Ok(sonuc);
    }

    /// <summary>POST /auth/logout — Oturum sonlandırma</summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        await _logoutCommand.ExecuteAsync(token);
        return Ok(new { mesaj = "Oturum başarıyla sonlandırıldı." });
    }

    /// <summary>POST /auth/forgot-password — Şifre sıfırlama token'ı oluştur</summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest istek,
        [FromServices] IUserRepository userRepository)
    {
        var kullanici = await userRepository.GetByEmailAsync(istek.Email);
        if (kullanici is null)
            return BadRequest(new { mesaj = "Bu e-posta adresine ait bir kullanıcı bulunamadı." });

        kullanici.ResetToken        = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        kullanici.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

        await userRepository.UpdateAsync(kullanici.Id, kullanici);

        return Ok(new { mesaj = "Şifre sıfırlama bağlantısı oluşturuldu.", resetToken = kullanici.ResetToken });
    }

    /// <summary>POST /auth/reset-password — Yeni şifre belirleme</summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest istek,
        [FromServices] IUserRepository userRepository,
        [FromServices] IPasswordHasher passwordHasher)
    {
        var kullanici = await userRepository.GetByResetTokenAsync(istek.Token);

        if (kullanici is null || kullanici.ResetTokenExpires < DateTime.UtcNow)
            return BadRequest(new { mesaj = "Geçersiz veya süresi dolmuş token." });

        kullanici.PasswordHash      = passwordHasher.Hash(istek.YeniSifre);
        kullanici.ResetToken        = null;
        kullanici.ResetTokenExpires = null;

        await userRepository.UpdateAsync(kullanici.Id, kullanici);

        return Ok(new { mesaj = "Şifreniz başarıyla güncellendi." });
    }
}
