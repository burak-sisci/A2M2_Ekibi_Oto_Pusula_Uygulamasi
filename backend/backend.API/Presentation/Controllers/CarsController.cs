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
    private readonly AddCarCommand _addCarCommand;
    private readonly IMediator _mediator;
 
    public CarsController(GetCarsQuery getCarsQuery, IMediator mediator, AddCarCommand addCarCommand)
    {
        _getCarsQuery = getCarsQuery;
        _addCarCommand = addCarCommand;
        _mediator = mediator;
    }
 
    /// <summary>Araçları listele (pagination + filtre)</summary>
    [HttpGet]
    public async Task<IActionResult> GetCars(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0,
        [FromQuery] string? brand = null,
        [FromQuery] string? seri = null,
        [FromQuery] string? images = null,
        [FromQuery] string? model = null,
        [FromQuery] string? location = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null
        )
    {
        var pagination = new PaginationParameters { Limit = limit, Offset = offset };
        var filter = new CarsFilter(brand, seri,images, model,location, minPrice, maxPrice);
        var result = await _getCarsQuery.ExecuteAsync(filter, pagination);
        return Ok(result);
    }
 
    /// <summary>Yeni araç ekle (auth zorunlu)</summary>
    //[Authorize]
    [HttpPost]
    public async Task<IActionResult> AddCar([FromBody] AddCarRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var car = await _addCarCommand.ExecuteAsync(request, userId);
        return StatusCode(201, car);
    }
 
    /// <summary>Araç bilgilerini güncelle (auth zorunlu ve sadece ilan sahibi)</summary>
    //[Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCar(string id, [FromBody] UpdateCarRequest request, [FromServices] ICarRepository carRepository)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var existingCar = await carRepository.GetByIdAsync(id);
 
        if (existingCar == null)
            return NotFound(new { message = "Araç bulunamadı." });
 
        // Sadece aracın sahibi güncelleyebilir
        if (existingCar.IlanSahibi != userId)
            return Forbid();
 
        existingCar.Marka        = request.Marka;
        existingCar.Resimler     = request.Resimler;
        existingCar.Model        = request.Model;
        existingCar.Yil          = request.Yil;
        existingCar.Fiyat        = request.Fiyat;
        existingCar.Konum        = request.Konum;
 
        var success = await carRepository.UpdateAsync(id, existingCar);
        if (!success)
            return BadRequest(new { message = "Araç güncellenirken bir hata oluştu." });
 
        return Ok(existingCar);
    }
 
    /// <summary>Araç sil (auth zorunlu ve sadece ilan sahibi)</summary>
    //[Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        // Command içine hem araba id'sini hem de silmeye çalışan kullanıcı id'sini gönder.
        var command = new DeleteCarCommand(id); 
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { Message = "Araba bulunamadı veya silme yetkiniz yok." });

        return Ok(new { Message = "Araba ve bu arabaya ait tüm yorumlar başarıyla silindi." });
    }
}