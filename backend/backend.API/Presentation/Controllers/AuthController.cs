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
        LogoutUserCommand logoutCommand,IMediator mediator)
    {
        _registerCommand = registerCommand;
        _loginCommand = loginCommand;
        _logoutCommand = logoutCommand;
        _mediator = mediator;
    }

    public record UpdateProfileRequestDto(string? Phone);

    /// <summary>Yeni kullanıcı kaydı</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _registerCommand.ExecuteAsync(request);
        return StatusCode(201, result);
    }

    /// <summary>Kullanıcı girişi</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _loginCommand.ExecuteAsync(request);
        return Ok(result);
    }

    /// <summary>Şifre sıfırlama talebi oluştur (Token üretir)</summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, [FromServices] IUserRepository userRepository)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new { message = "Bu e-posta adresine ait bir kullanıcı bulunamadı." });

        user.ResetToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

        await userRepository.UpdateAsync(user.Id, user);

        return Ok(new { message = "Şifre sıfırlama bağlantısı oluşturuldu.", resetToken = user.ResetToken });
    }

    /// <summary>Token ile yeni şifreyi belirle</summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, [FromServices] IUserRepository userRepository, [FromServices] IPasswordHasher passwordHasher)
    {
        var user = await userRepository.GetByResetTokenAsync(request.Token);
        
        if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
            return BadRequest(new { message = "Geçersiz veya süresi dolmuş token." });

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        
        user.ResetToken = null;
        user.ResetTokenExpires = null;

        await userRepository.UpdateAsync(user.Id, user);

        return Ok(new { message = "Şifreniz başarıyla güncellendi." });
    }

     /// <summary>Kullanıcı profili güncelleme</summary>
    [Authorize] 
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
    {
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Yetkisiz işlem." });

        var command = new UpdateProfileCommand(
            userId,  
            request.Phone
        );

        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = "Kullanıcı bulunamadı." });

        return Ok(new { message = "Profiliniz başarıyla güncellendi." });
    }


    /// <summary>Kullanıcı çıkışı - token blacklist'e eklenir</summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        await _logoutCommand.ExecuteAsync(token);
        return Ok(new { message = "Başarıyla çıkış yapıldı." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var command = new DeleteUserCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
        {
            return NotFound(new { Message = "Kullanıcı bulunamadı." });
        }

        return Ok(new { Message = "Kullanıcı ve ona ait tüm veriler (arabalar, yorumlar) başarıyla silindi." });
    }
}

