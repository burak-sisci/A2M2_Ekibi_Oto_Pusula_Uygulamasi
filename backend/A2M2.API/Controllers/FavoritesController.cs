using System.Security.Claims;
using A2M2.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A2M2.API.Controllers;

/// <summary>
/// Favori controller'ı — Ekleme, Silme, Listeleme (tümü Protected)
/// </summary>
[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FavoriteService _favoriteService;

    public FavoritesController(FavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    /// <summary>GET /api/favorites</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
        return Ok(favorites);
    }

    /// <summary>POST /api/favorites/{listingId}</summary>
    [HttpPost("{listingId}")]
    public async Task<IActionResult> Add(string listingId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var favorite = await _favoriteService.AddAsync(userId, listingId);
            return Ok(favorite);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>DELETE /api/favorites/{listingId}</summary>
    [HttpDelete("{listingId}")]
    public async Task<IActionResult> Remove(string listingId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var removed = await _favoriteService.RemoveAsync(userId, listingId);
        if (!removed) return NotFound(new { message = "Favori bulunamadı" });
        return Ok(new { message = "Favori kaldırıldı" });
    }
}
