using System.Security.Claims;
using A2M2.API.DTOs;
using A2M2.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A2M2.API.Controllers;

/// <summary>
/// Yorum controller'ı — Modül D: Etkileşim ve İletişim
/// </summary>
[ApiController]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly CommentService _commentService;

    public CommentsController(CommentService commentService)
    {
        _commentService = commentService;
    }

    /// <summary>POST /api/cars/{carId}/comments — Yorum ekle</summary>
    [HttpPost("api/cars/{carId}/comments")]
    public async Task<IActionResult> Create(string carId, [FromBody] CreateCommentRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var comment = await _commentService.CreateAsync(carId, userId, request);
        return CreatedAtAction(nameof(GetByCarId), new { carId }, comment);
    }

    /// <summary>GET /api/cars/{carId}/comments — İlan yorumlarını listele</summary>
    [AllowAnonymous]
    [HttpGet("api/cars/{carId}/comments")]
    public async Task<IActionResult> GetByCarId(string carId)
    {
        var comments = await _commentService.GetByCarAsync(carId);
        return Ok(comments);
    }

    /// <summary>PUT /api/comments/{commentId} — Yorum güncelle (sadece sahibi)</summary>
    [HttpPut("api/comments/{commentId}")]
    public async Task<IActionResult> Update(string commentId, [FromBody] UpdateCommentRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var comment = await _commentService.UpdateAsync(commentId, userId, request);
            if (comment == null) return NotFound(new { message = "Yorum bulunamadı" });
            return Ok(comment);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    /// <summary>DELETE /api/comments/{commentId} — Yorum sil (sadece sahibi)</summary>
    [HttpDelete("api/comments/{commentId}")]
    public async Task<IActionResult> Delete(string commentId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var deleted = await _commentService.DeleteAsync(commentId, userId);
            if (!deleted) return NotFound(new { message = "Yorum bulunamadı" });
            return Ok(new { message = "Yorum silindi" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>GET /api/users/{userId}/comments — Kullanıcının tüm yorumları</summary>
    [AllowAnonymous]
    [HttpGet("api/users/{userId}/comments")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var comments = await _commentService.GetByUserAsync(userId);
        return Ok(comments);
    }
}
