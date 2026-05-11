using System.Security.Claims;
using backend.API.Modules.Auth.Application;
using backend.API.Modules.Comments.Application;
using backend.API.Modules.Lists.Application;
using backend.API.Shared.Paginition;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Presentation.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;
    private readonly IListRepository _listRepository;
    private readonly ICommentRepository _commentRepository;

    public UsersController(
        IMediator mediator,
        IUserRepository userRepository,
        IListRepository listRepository,
        ICommentRepository commentRepository)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _listRepository = listRepository;
        _commentRepository = commentRepository;
    }

    /// <summary>GET /users/{userId} — Profil görüntüleme</summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return NotFound(new { mesaj = "Kullanıcı bulunamadı." });

        return Ok(new
        {
            id          = user.Id,
            ad          = user.Ad,
            email       = user.Email,
            telefon     = user.Phone,
            cinsiyet    = user.Cinsiyet?.ToString(),
            dogumTarihi = user.DogumTarihi,
            kayitTarihi = user.CreatedAt
        });
    }

    /// <summary>PUT /users/{userId} — Profil güncelleme (sadece kendi profili)</summary>
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] ProfilGuncelleIstek istek)
    {
        var istekciId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var komut = new UpdateUserCommand(userId, istekciId, istek.Ad, istek.Telefon, istek.YeniSifre);

        var basarili = await _mediator.Send(komut);
        if (!basarili)
            return NotFound(new { mesaj = "Kullanıcı bulunamadı." });

        var guncellenmis = await _userRepository.GetByIdAsync(userId);
        return Ok(new
        {
            id      = guncellenmis!.Id,
            ad      = guncellenmis.Ad,
            email   = guncellenmis.Email,
            telefon = guncellenmis.Phone
        });
    }

    /// <summary>DELETE /users/{userId} — Hesap silme (sadece kendi hesabı)</summary>
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var istekciId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (userId != istekciId)
            return StatusCode(403, new { mesaj = "Bu hesabı silme yetkiniz yok." });

        var komut = new DeleteUserCommand(userId);
        var basarili = await _mediator.Send(komut);

        if (!basarili)
            return NotFound(new { mesaj = "Kullanıcı bulunamadı." });

        return NoContent();
    }

    /// <summary>GET /users/{userId}/lists — Kullanıcının listelerini görüntüleme</summary>
    [HttpGet("{userId}/lists")]
    public async Task<IActionResult> GetUserLists(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return NotFound(new { mesaj = "Kullanıcı bulunamadı." });

        var listeler = await _listRepository.GetByUserIdAsync(userId);
        var sonuc = listeler.Select(l => new
        {
            id          = l.Id,
            ad          = l.ListName,
            varsayilan  = l.IsDefault,
            ilanSayisi  = l.Items.Count,
            kayitTarihi = l.CreatedAt
        });

        return Ok(sonuc);
    }

    /// <summary>GET /users/{userId}/comments — Kullanıcının yorumlarını listeleme</summary>
    [HttpGet("{userId}/comments")]
    public async Task<IActionResult> GetUserComments(
        string userId,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return NotFound(new { mesaj = "Kullanıcı bulunamadı." });

        var sayfalama = new PaginationParameters { Limit = limit, Offset = offset };
        var yorumlar  = await _commentRepository.GetByUserIdAsync(userId, sayfalama);

        return Ok(yorumlar);
    }
}

public class ProfilGuncelleIstek { public string? Ad { get; set; } public string? Telefon { get; set; } public string? YeniSifre { get; set; } }
