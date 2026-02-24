using System.Security.Claims;
using A2M2.API.DTOs;
using A2M2.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A2M2.API.Controllers;

/// <summary>
/// İlan controller'ı — CRUD + filtreleme + cache
/// </summary>
[ApiController]
[Route("api/listings")]
public class ListingsController : ControllerBase
{
    private readonly ListingService _listingService;
    private readonly RedisCacheService _cacheService;

    public ListingsController(ListingService listingService, RedisCacheService cacheService)
    {
        _listingService = listingService;
        _cacheService = cacheService;
    }

    /// <summary>GET /api/listings (cache: 5 dk)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ListingFilterParams filters)
    {
        // Cache kontrolü
        var cacheKey = $"listings:{Request.QueryString}";
        var cached = await _cacheService.GetAsync<object>(cacheKey);
        if (cached != null) return Ok(cached);

        var listings = await _listingService.GetAllAsync(filters);
        await _cacheService.SetAsync(cacheKey, listings, TimeSpan.FromMinutes(5));
        return Ok(listings);
    }

    /// <summary>GET /api/listings/{id} (cache: 2 dk)</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var cacheKey = $"listing:{id}";
        var cached = await _cacheService.GetAsync<object>(cacheKey);
        if (cached != null) return Ok(cached);

        var listing = await _listingService.GetByIdAsync(id);
        if (listing == null) return NotFound(new { message = "İlan bulunamadı" });

        await _cacheService.SetAsync(cacheKey, listing, TimeSpan.FromMinutes(2));
        return Ok(listing);
    }

    /// <summary>POST /api/listings (Protected)</summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListingRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var listing = await _listingService.CreateAsync(request, userId);

        // Cache invalidation
        await _cacheService.DeletePatternAsync("listings:*");

        return CreatedAtAction(nameof(GetById), new { id = listing.Id }, listing);
    }

    /// <summary>PUT /api/listings/{id} (Protected, owner-only)</summary>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateListingRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var listing = await _listingService.UpdateAsync(id, request, userId);
            if (listing == null) return NotFound(new { message = "İlan bulunamadı" });

            // Cache invalidation
            await _cacheService.DeletePatternAsync("listings:*");
            await _cacheService.DeleteAsync($"listing:{id}");

            return Ok(listing);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    /// <summary>DELETE /api/listings/{id} (Protected, owner-only)</summary>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var deleted = await _listingService.DeleteAsync(id, userId);
            if (!deleted) return NotFound(new { message = "İlan bulunamadı" });

            // Cache invalidation
            await _cacheService.DeletePatternAsync("listings:*");
            await _cacheService.DeleteAsync($"listing:{id}");

            return Ok(new { message = "İlan silindi" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
