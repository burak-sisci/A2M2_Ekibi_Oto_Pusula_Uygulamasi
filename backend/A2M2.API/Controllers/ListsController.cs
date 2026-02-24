using System.Security.Claims;
using A2M2.API.DTOs;
using A2M2.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A2M2.API.Controllers;

/// <summary>
/// Liste controller'ı — Modül C: Özel Listeler ve Favoriler
/// </summary>
[ApiController]
[Authorize]
public class ListsController : ControllerBase
{
    private readonly ListService _listService;

    public ListsController(ListService listService)
    {
        _listService = listService;
    }

    /// <summary>POST /api/lists — Yeni liste oluştur</summary>
    [HttpPost("api/lists")]
    public async Task<IActionResult> Create([FromBody] CreateListRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var list = await _listService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetDetail), new { listId = list.Id }, list);
    }

    /// <summary>POST /api/lists/{listId}/cars — Listeye ilan ekle</summary>
    [HttpPost("api/lists/{listId}/cars")]
    public async Task<IActionResult> AddCar(string listId, [FromBody] AddCarToListRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var list = await _listService.AddCarAsync(listId, userId, request.CarId);
            if (list == null) return NotFound(new { message = "Liste bulunamadı" });
            return Ok(list);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>GET /api/users/{userId}/lists — Kullanıcının listelerini getir</summary>
    [HttpGet("api/users/{userId}/lists")]
    public async Task<IActionResult> GetUserLists(string userId)
    {
        var lists = await _listService.GetUserListsAsync(userId);
        return Ok(lists);
    }

    /// <summary>GET /api/lists/{listId} — Liste içeriğini getir</summary>
    [HttpGet("api/lists/{listId}")]
    public async Task<IActionResult> GetDetail(string listId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var detail = await _listService.GetListDetailAsync(listId, userId);
        if (detail == null) return NotFound(new { message = "Liste bulunamadı" });
        return Ok(detail);
    }

    /// <summary>PUT /api/lists/{listId} — Liste ismini güncelle (Favoriler hariç)</summary>
    [HttpPut("api/lists/{listId}")]
    public async Task<IActionResult> UpdateName(string listId, [FromBody] UpdateListRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var list = await _listService.UpdateNameAsync(listId, userId, request);
            if (list == null) return NotFound(new { message = "Liste bulunamadı" });
            return Ok(list);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>DELETE /api/lists/{listId}/cars/{carId} — Listeden ilan çıkar</summary>
    [HttpDelete("api/lists/{listId}/cars/{carId}")]
    public async Task<IActionResult> RemoveCar(string listId, string carId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var removed = await _listService.RemoveCarAsync(listId, userId, carId);
        if (!removed) return NotFound(new { message = "Liste veya araç bulunamadı" });
        return Ok(new { message = "Araç listeden çıkarıldı" });
    }

    /// <summary>DELETE /api/lists/{listId} — Listeyi tamamen sil</summary>
    [HttpDelete("api/lists/{listId}")]
    public async Task<IActionResult> Delete(string listId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var deleted = await _listService.DeleteAsync(listId, userId);
            if (!deleted) return NotFound(new { message = "Liste bulunamadı" });
            return Ok(new { message = "Liste silindi" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
