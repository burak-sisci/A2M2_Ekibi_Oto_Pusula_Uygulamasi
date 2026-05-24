using backend.API.Modules.Auth.Application;
using backend.API.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace backend.API.Presentation.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserCommand   _registerCommand;
    private readonly LoginUserCommand      _loginCommand;
    private readonly LogoutUserCommand     _logoutCommand;
    private readonly RefreshTokenCommand   _refreshCommand;
    private readonly IRabbitMqPublisher    _publisher;
    private readonly IMediator             _mediator;

    public AuthController(
        RegisterUserCommand registerCommand,
        LoginUserCommand loginCommand,
        LogoutUserCommand logoutCommand,
        RefreshTokenCommand refreshCommand,
        IRabbitMqPublisher publisher,
        IMediator mediator)
    {
        _registerCommand = registerCommand;
        _loginCommand    = loginCommand;
        _logoutCommand   = logoutCommand;
        _refreshCommand  = refreshCommand;
        _publisher       = publisher;
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

    /// <summary>POST /auth/refresh — Refresh token ile yeni access üret (rotation).</summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest istek)
    {
        if (string.IsNullOrWhiteSpace(istek.RefreshToken))
            return BadRequest(new { mesaj = "refreshToken zorunlu." });

        try
        {
            var sonuc = await _refreshCommand.ExecuteAsync(istek.RefreshToken);
            return Ok(sonuc);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mesaj = ex.Message });
        }
    }

    /// <summary>POST /auth/logout — Oturum sonlandırma. Body'de refreshToken opsiyonel.</summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest? istek = null)
    {
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        await _logoutCommand.ExecuteAsync(token, istek?.RefreshToken);
        return Ok(new { mesaj = "Oturum başarıyla sonlandırıldı." });
    }

    /// <summary>POST /auth/forgot-password — Şifre sıfırlama token'ı oluştur + email kuyruğa at.</summary>
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

        // Notification Service (Faz 2.5) bu kuyruğu tüketip mail gönderecek.
        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000";
        var resetUrl    = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(kullanici.ResetToken)}";

        await _publisher.PublishAsync(new EmailSendEvent(
            To:         kullanici.Email,
            TemplateId: "password-reset",
            Variables:  new Dictionary<string, string>
            {
                ["UserName"] = kullanici.Ad,
                ["ResetUrl"] = resetUrl
            }
        ), RabbitMqQueues.EmailSend);

        return Ok(new { mesaj = "Şifre sıfırlama bağlantısı e-postanıza gönderildi.", resetToken = kullanici.ResetToken });
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

public record LogoutRequest(string? RefreshToken = null);
