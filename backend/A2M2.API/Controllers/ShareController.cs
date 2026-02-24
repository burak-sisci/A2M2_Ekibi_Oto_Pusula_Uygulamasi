using A2M2.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace A2M2.API.Controllers;

/// <summary>
/// Paylaşım controller'ı — Modül D: GET /cars/{carId}/share
/// </summary>
[ApiController]
[Route("api/cars")]
public class ShareController : ControllerBase
{
    private readonly ListingService _listingService;
    private readonly IConfiguration _config;

    public ShareController(ListingService listingService, IConfiguration config)
    {
        _listingService = listingService;
        _config = config;
    }

    /// <summary>GET /api/cars/{carId}/share — Paylaşım linki üret</summary>
    [HttpGet("{carId}/share")]
    public async Task<IActionResult> GetShareInfo(string carId)
    {
        var listing = await _listingService.GetByIdAsync(carId);
        if (listing == null) return NotFound(new { message = "İlan bulunamadı" });

        var frontendUrl = _config["AllowedOrigins:0"] ?? "http://localhost:5173";

        return Ok(new
        {
            title = $"{listing.Brand} {listing.Model} ({listing.Year})",
            description = $"{listing.Km:N0} km — {listing.Price:N0} ₺",
            url = $"{frontendUrl}/cars/{listing.Id}",
        });
    }
}
