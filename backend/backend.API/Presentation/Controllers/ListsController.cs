using System.Security.Claims;
using backend.API.Modules.Lists.Application;
using backend.API.Modules.Lists.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Presentation.Controllers;

[ApiController]
[Route("lists")]
[Authorize]
public class ListsController : ControllerBase
{
    private readonly IListRepository _listRepository;
    private readonly IListCacheService _cache;
    private readonly CreateListCommand _createListCommand;
    private readonly AddCarToListCommand _addCarCommand;
    private readonly RemoveCarFromListCommand _removeCarCommand;
    private readonly UpdateListNameCommand _updateNameCommand;
    private readonly DeleteListCommand _deleteListCommand;

    public ListsController(
        IListRepository listRepository,
        IListCacheService cache,
        CreateListCommand createListCommand,
        AddCarToListCommand addCarCommand,
        RemoveCarFromListCommand removeCarCommand,
        UpdateListNameCommand updateNameCommand,
        DeleteListCommand deleteListCommand)
    {
        _listRepository   = listRepository;
        _cache            = cache;
        _createListCommand   = createListCommand;
        _addCarCommand       = addCarCommand;
        _removeCarCommand    = removeCarCommand;
        _updateNameCommand   = updateNameCommand;
        _deleteListCommand   = deleteListCommand;
    }

    /// <summary>POST /lists — Yeni liste oluştur</summary>
    [HttpPost]
    public async Task<IActionResult> CreateList([FromBody] ListeOlusturIstek istek)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var liste  = await _createListCommand.ExecuteAsync(userId, istek.ListeAdi);
        return StatusCode(201, MapToDto(liste));
    }

    /// <summary>GET /lists/{listId} — Liste içeriğini görüntüle</summary>
    [HttpGet("{listId}")]
    public async Task<IActionResult> GetList(string listId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var liste  = await _listRepository.GetByIdAsync(listId);

        if (liste is null)
            return NotFound(new { mesaj = "Liste bulunamadı." });

        if (liste.UserId != userId)
            return StatusCode(403, new { mesaj = "Bu listeye erişim yetkiniz yok." });

        return Ok(new
        {
            id          = liste.Id,
            ad          = liste.ListName,
            varsayilan  = liste.IsDefault,
            ilanlar     = liste.Items,
            kayitTarihi = liste.CreatedAt,
            guncelleme  = liste.UpdatedAt
        });
    }

    /// <summary>PUT /lists/{listId} — Liste adını güncelle (Favoriler değiştirilemez)</summary>
    [HttpPut("{listId}")]
    public async Task<IActionResult> UpdateList(string listId, [FromBody] ListeGuncelleIstek istek)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var basarili = await _updateNameCommand.ExecuteAsync(listId, userId, istek.YeniAd);

        if (!basarili)
            return NotFound(new { mesaj = "Liste bulunamadı." });

        var guncellenmis = await _listRepository.GetByIdAsync(listId);
        return Ok(MapToDto(guncellenmis!));
    }

    /// <summary>DELETE /lists/{listId} — Listeyi sil (Favoriler silinemez)</summary>
    [HttpDelete("{listId}")]
    public async Task<IActionResult> DeleteList(string listId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var basarili = await _deleteListCommand.ExecuteAsync(listId, userId);

        if (!basarili)
            return NotFound(new { mesaj = "Liste bulunamadı veya silme yetkiniz yok." });

        return NoContent();
    }

    /// <summary>POST /lists/{listId}/cars — Listeye ilan ekle (RabbitMQ event + Redis invalidation)</summary>
    [HttpPost("{listId}/cars")]
    public async Task<IActionResult> AddCarToList(string listId, [FromBody] ListeIlanEkleIstek istek)
    {
        var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var basarili = await _addCarCommand.ExecuteAsync(listId, istek.IlanId, userId);

        if (!basarili)
            return NotFound(new { mesaj = "Liste veya ilan bulunamadı." });

        var guncellenmis = await _listRepository.GetByIdAsync(listId);
        return Ok(MapToDto(guncellenmis!));
    }

    /// <summary>DELETE /lists/{listId}/cars/{carId} — Listeden ilan çıkar</summary>
    [HttpDelete("{listId}/cars/{carId}")]
    public async Task<IActionResult> RemoveCarFromList(string listId, string carId)
    {
        var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var basarili = await _removeCarCommand.ExecuteAsync(listId, carId, userId);

        if (!basarili)
            return NotFound(new { mesaj = "Liste veya ilan bulunamadı." });

        var guncellenmis = await _listRepository.GetByIdAsync(listId);
        return Ok(MapToDto(guncellenmis!));
    }

    private static object MapToDto(UserList liste) => new
    {
        id          = liste.Id,
        ad          = liste.ListName,
        varsayilan  = liste.IsDefault,
        ilanSayisi  = liste.Items.Count,
        kayitTarihi = liste.CreatedAt,
        guncelleme  = liste.UpdatedAt
    };
}

public class ListeOlusturIstek   { public string ListeAdi { get; set; } = string.Empty; }
public class ListeGuncelleIstek  { public string YeniAd  { get; set; } = string.Empty; }
public class ListeIlanEkleIstek  { public string IlanId  { get; set; } = string.Empty; }
