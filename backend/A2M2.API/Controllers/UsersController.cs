using System.Security.Claims;
using A2M2.API.DTOs;
using A2M2.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A2M2.API.Controllers;

/// <summary>
/// Kullanıcı profil controller'ı — Görüntüleme, Güncelleme, Silme (Modül A)
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>GET /api/users/{userId} — Profil görüntüleme</summary>
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProfile(string userId)
    {
        var profile = await _userService.GetProfileAsync(userId);
        if (profile == null) return NotFound(new { message = "Kullanıcı bulunamadı" });
        return Ok(profile);
    }

    /// <summary>PUT /api/users/{userId} — Profil güncelleme (sadece kendi profili)</summary>
    [Authorize]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateProfile(string userId, [FromBody] UpdateUserRequest request)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != userId)
            return Forbid();

        var profile = await _userService.UpdateProfileAsync(userId, request);
        if (profile == null) return NotFound(new { message = "Kullanıcı bulunamadı" });
        return Ok(profile);
    }

    /// <summary>DELETE /api/users/{userId} — Hesap silme (sadece kendi hesabı)</summary>
    [Authorize]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteAccount(string userId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != userId)
            return Forbid();

        var deleted = await _userService.DeleteAccountAsync(userId);
        if (!deleted) return NotFound(new { message = "Kullanıcı bulunamadı" });
        return Ok(new { message = "Hesap silindi" });
    }
}
