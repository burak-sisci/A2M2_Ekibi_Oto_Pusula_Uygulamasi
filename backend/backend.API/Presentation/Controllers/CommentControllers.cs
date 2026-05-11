using System.Security.Claims;
using backend.API.Modules.Comments.Application;
using backend.API.Shared.Paginition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Presentation.Controllers;

// ── /cars/{carId}/comments ─────────────────────────────────────────────────────
[ApiController]
[Route("cars/{carId}/comments")]
public class CommentsController : ControllerBase
{
    private readonly GetCarCommentsQuery _getCommentsQuery;
    private readonly AddCommentCommand _addCommentCommand;
    private readonly IShareLinkService _shareLinkService;

    public CommentsController(
        GetCarCommentsQuery getCommentsQuery,
        AddCommentCommand addCommentCommand,
        IShareLinkService shareLinkService)
    {
        _getCommentsQuery  = getCommentsQuery;
        _addCommentCommand = addCommentCommand;
        _shareLinkService  = shareLinkService;
    }

    /// <summary>GET /cars/{carId}/comments — İlan yorumlarını listele</summary>
    [HttpGet]
    public async Task<IActionResult> GetComments(
        string carId,
        [FromQuery] int limit  = 10,
        [FromQuery] int offset = 0)
    {
        var sayfalama = new PaginationParameters { Limit = limit, Offset = offset };
        var yorumlar  = await _getCommentsQuery.ExecuteAsync(carId, sayfalama);
        return Ok(yorumlar);
    }

    /// <summary>POST /cars/{carId}/comments — Yorum ekle (RabbitMQ event)</summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComment(string carId, [FromBody] YorumEkleIstek istek)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var request = new AddCommentRequest(carId, istek.Icerik);
        var yorum   = await _addCommentCommand.ExecuteAsync(request, userId);
        return StatusCode(201, yorum);
    }

    /// <summary>GET /cars/{carId}/share — Paylaşım linki üret (Redis cache 90 gün)</summary>
    [Authorize]
    [HttpGet("/cars/{carId}/share")]
    public async Task<IActionResult> GetShareLink(string carId)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var sonuc   = await _shareLinkService.GetOrCreateAsync(carId, baseUrl);
        return Ok(sonuc);
    }
}

// ── /comments/{commentId} ──────────────────────────────────────────────────────
[ApiController]
[Route("comments")]
[Authorize]
public class IndependentCommentsController : ControllerBase
{
    private readonly UpdateCommentCommand _updateCommand;
    private readonly DeleteCommentCommand _deleteCommand;
    private readonly LikeCommentCommand _likeCommand;
    private readonly UnlikeCommentCommand _unlikeCommand;

    public IndependentCommentsController(
        UpdateCommentCommand updateCommand,
        DeleteCommentCommand deleteCommand,
        LikeCommentCommand likeCommand,
        UnlikeCommentCommand unlikeCommand)
    {
        _updateCommand = updateCommand;
        _deleteCommand = deleteCommand;
        _likeCommand   = likeCommand;
        _unlikeCommand = unlikeCommand;
    }

    /// <summary>PUT /comments/{commentId} — Yorum güncelle (sadece yorum sahibi)</summary>
    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateComment(string commentId, [FromBody] YorumGuncelleIstek istek)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var request = new UpdateCommentRequest(istek.Icerik, commentId);
        var yorum   = await _updateCommand.ExecuteAsync(request, userId);
        return Ok(yorum);
    }

    /// <summary>DELETE /comments/{commentId} — Yorum sil (sadece yorum sahibi)</summary>
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(string commentId)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var request = new DeleteCommentRequest(commentId, userId);
        await _deleteCommand.ExecuteAsync(request);
        return NoContent();
    }

    /// <summary>POST /comments/{commentId}/like — Yorum beğen</summary>
    [HttpPost("{commentId}/like")]
    public async Task<IActionResult> LikeComment(string commentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var sonuc  = await _likeCommand.ExecuteAsync(commentId, userId);
        return Ok(sonuc);
    }

    /// <summary>DELETE /comments/{commentId}/like — Beğeniyi geri al</summary>
    [HttpDelete("{commentId}/like")]
    public async Task<IActionResult> UnlikeComment(string commentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var sonuc  = await _unlikeCommand.ExecuteAsync(commentId, userId);
        return Ok(sonuc);
    }
}

public class YorumEkleIstek   { public string Icerik { get; set; } = string.Empty; }
public class YorumGuncelleIstek { public string Icerik { get; set; } = string.Empty; }
