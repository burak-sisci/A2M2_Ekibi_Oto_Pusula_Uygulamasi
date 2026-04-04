using System.Security.Claims;
using backend.API.Modules.Comments.Application;
using backend.API.Shared.Paginition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Presentation.Controllers;

[ApiController]
[Route("cars/{carId}/comments")]
public class CommentsController : ControllerBase
{
    private readonly GetCarCommentsQuery _getCommentsQuery;
    private readonly GetCommentQuery _getCommentQuery;
    private readonly AddCommentCommand _addCommentCommand;
    private readonly UpdateCommentCommand _updateCommentCommand;
    private readonly DeleteCommentCommand _deleteCommentCommand;

    public CommentsController(GetCarCommentsQuery getCommentsQuery, AddCommentCommand addCommentCommand, GetCommentQuery getCommentQuery, UpdateCommentCommand updateCommentCommand, DeleteCommentCommand deleteCommentCommand)
    {
        _getCommentsQuery = getCommentsQuery;
        _getCommentQuery = getCommentQuery;
        _addCommentCommand = addCommentCommand;
        _updateCommentCommand = updateCommentCommand;
        _deleteCommentCommand = deleteCommentCommand;
    }

    /// <summary>Araca ait yorumları getir (pagination zorunlu)</summary>
    [HttpGet]// Route: GET /cars/{carId}/comments
    public async Task<IActionResult> GetComments(
        string carId,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var pagination = new PaginationParameters { Limit = limit, Offset = offset };
        var result = await _getCommentsQuery.ExecuteAsync(carId, pagination);
        return Ok(result);
    }

    /// <summary>Belirli arac yorumunu getir (auth zorunlu-kullanıcıya özel)</summary>
    [Authorize]
    [HttpGet("{id}")]// Route: GET /cars/{carId}/comments/{id}
    public async Task<IActionResult> GetComment(string id)
    {
        var comment = await _getCommentQuery.ExecuteAsync(id);
        return StatusCode(200, comment);
    }

    /// <summary>Araca yorum ekle (auth zorunlu)</summary>
    [Authorize]
    [HttpPost]// Route: POST /cars/{carId}/comments
    public async Task<IActionResult> AddComment(string carId, [FromBody] AddCommentBodyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var commentRequest = new AddCommentRequest(CarId:carId, Content:request.Content);
        var comment = await _addCommentCommand.ExecuteAsync(commentRequest, userId);
        return StatusCode(201, comment);
    }

    /// <summary>Arac yorumunu güncelle (auth zorunlu)</summary>
    [Authorize]
    [HttpPut("{id}")]// Route: PUT /cars/{carId}/comments/{id}
    public async Task<IActionResult> UpdateComment(string id, [FromBody] UpdateCommentBodyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var updateRequest = new UpdateCommentRequest(Id:id,Content: request.Content);
        var update = await _updateCommentCommand.ExecuteAsync(updateRequest, userId);
        return StatusCode(200, update);
    }

    /// <summary>Arac yorumunu sil (auth zorunlu)</summary>
    [Authorize]
    [HttpDelete("{id}")]// Route: DELETE /cars/{carId}/comments/{id}
    public async Task<IActionResult> DeleteComment(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var deleteRequest = new DeleteCommentRequest(id, userId);
        var delete = await _deleteCommentCommand.ExecuteAsync(deleteRequest);
        return StatusCode(200, delete);
    }
}

public record AddCommentBodyRequest(string Content);
public record UpdateCommentBodyRequest(string Content);
