using System.Security.Claims;
using backend.API.Modules.Lists.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Presentation.Controllers;

[ApiController]
[Route("lists")]
[Authorize]
public class ListsController : ControllerBase
{
    private readonly IListRepository _listRepository;
    private readonly AddItemToListCommand _addItemCommand;

    public ListsController(IListRepository listRepository, AddItemToListCommand addItemCommand)
    {
        _listRepository = listRepository;
        _addItemCommand = addItemCommand;
    }

    /// <summary>Kullanıcının listelerini getir</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyLists()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var lists = await _listRepository.GetByUserIdAsync(userId);
        return Ok(lists);
    }

    /// <summary>Yeni liste oluştur</summary>
    [HttpPost]
    public async Task<IActionResult> CreateList([FromBody] CreateListRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var list = new Modules.Lists.Domain.UserList
        {
            UserId = userId,
            ListName = request.ListName
        };
        await _listRepository.CreateAsync(list);
        return StatusCode(201, list);
    }

    /// <summary>Listeye araç ekle</summary>
    [HttpPut("{id}/items")]
    public async Task<IActionResult> AddItem(string id, [FromBody] AddItemRequest request)
    {
        var success = await _addItemCommand.ExecuteAsync(id, request.CarId);
        if (!success) return NotFound(new { message = "Liste bulunamadı." });
        return Ok(new { message = "Araç listeye eklendi." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteList(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        var success = await _listRepository.DeleteAsync(id, userId);
        
        if (!success) 
        {
            return NotFound(new { message = "Liste bulunamadı veya bu listeyi silme yetkiniz yok." });
        }

        return Ok(new { message = "Liste başarıyla silindi." });
    }
}

public record CreateListRequest(string ListName);
public record AddItemRequest(string CarId);
