using System.Security.Claims;
using A2M2.API.DTOs;
using A2M2.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A2M2.API.Controllers;

/// <summary>
/// Kimlik doğrulama controller'ı — Register, Login, Logout, GetMe
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>POST /api/auth/register</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>POST /api/auth/login</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>POST /api/auth/logout</summary>
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // JWT stateless — istemci tarafında token silinir
        return Ok(new { message = "Çıkış başarılı" });
    }

    /// <summary>GET /api/auth/me</summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var user = await _authService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        return Ok(new { user.Id, user.Name, user.Email });
    }
}
