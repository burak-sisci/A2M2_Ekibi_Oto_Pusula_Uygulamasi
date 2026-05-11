using System.Security.Claims;
using backend.API.Modules.Cars.Application;
using backend.API.Shared.Paginition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace backend.API.Presentation.Controllers;

[ApiController]
[Route("cars")]
public class CarsController : ControllerBase
{
    private readonly GetCarsQuery _getCarsQuery;
    private readonly GetCarByIdQuery _getCarByIdQuery;
    private readonly AddCarCommand _addCarCommand;
    private readonly UpdateCarCommand _updateCarCommand;
    private readonly IMediator _mediator;

    public CarsController(
        GetCarsQuery getCarsQuery,
        GetCarByIdQuery getCarByIdQuery,
        AddCarCommand addCarCommand,
        UpdateCarCommand updateCarCommand,
        IMediator mediator)
    {
        _getCarsQuery    = getCarsQuery;
        _getCarByIdQuery = getCarByIdQuery;
        _addCarCommand   = addCarCommand;
        _updateCarCommand = updateCarCommand;
        _mediator        = mediator;
    }

    /// <summary>GET /cars — İlanları filtrele ve listele (Redis cache'li)</summary>
    [HttpGet]
    public async Task<IActionResult> GetCars(
        [FromQuery] int limit       = 20,
        [FromQuery] int offset      = 0,
        [FromQuery] string? brand   = null,
        [FromQuery] string? seri    = null,
        [FromQuery] string? images  = null,
        [FromQuery] string? model   = null,
        [FromQuery] string? location= null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        var sayfalama = new PaginationParameters { Limit = limit, Offset = offset };
        var filtre    = new CarsFilter(Marka: brand, Seri: seri, Model: model, Konum: location, MinFiyat: minPrice, MaxFiyat: maxPrice);
        var sonuc     = await _getCarsQuery.ExecuteAsync(filtre, sayfalama);
        return Ok(sonuc);
    }

    /// <summary>GET /cars/{carId} — İlan detayı görüntüleme</summary>
    [HttpGet("{carId}")]
    public async Task<IActionResult> GetCar(string carId)
    {
        var ilan = await _getCarByIdQuery.ExecuteAsync(carId);
        if (ilan is null)
            return NotFound(new { mesaj = "İlan bulunamadı." });

        return Ok(ilan);
    }

    /// <summary>POST /cars — Yeni ilan ekle (RabbitMQ + Redis invalidation)</summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddCar([FromBody] AddCarRequest istek)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ilan   = await _addCarCommand.ExecuteAsync(istek, userId);
        return StatusCode(201, ilan);
    }

    /// <summary>PUT /cars/{carId} — İlan güncelleme (sadece ilan sahibi)</summary>
    [Authorize]
    [HttpPut("{carId}")]
    public async Task<IActionResult> UpdateCar(
        string carId,
        [FromBody] UpdateCarRequest istek,
        [FromServices] ICarRepository carRepository)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var mevcut  = await carRepository.GetByIdAsync(carId);

        if (mevcut is null)
            return NotFound(new { mesaj = "İlan bulunamadı." });

        if (mevcut.IlanSahibi != userId)
            return StatusCode(403, new { mesaj = "Bu ilanı güncelleme yetkiniz yok." });

        var basarili = await _updateCarCommand.ExecuteAsync(carId, istek);
        if (!basarili)
            return BadRequest(new { mesaj = "İlan güncellenirken bir hata oluştu." });

        return Ok(await carRepository.GetByIdAsync(carId));
    }

    /// <summary>DELETE /cars/{carId} — İlan silme (sadece ilan sahibi)</summary>
    [Authorize]
    [HttpDelete("{carId}")]
    public async Task<IActionResult> DeleteCar(string carId)
    {
        var komut  = new DeleteCarCommand(carId);
        var sonuc  = await _mediator.Send(komut);

        if (!sonuc)
            return NotFound(new { mesaj = "İlan bulunamadı veya silme yetkiniz yok." });

        return Ok(new { mesaj = "İlan ve tüm yorumları başarıyla silindi." });
    }
}
