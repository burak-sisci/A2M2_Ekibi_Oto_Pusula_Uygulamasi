using backend.API.Modules.Cars.Domain;
using backend.API.Modules.Prediction.Application;
using backend.API.Modules.Prediction.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly IMediator _mediator;

    public PredictionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("predict")]
    [ProducesResponseType(typeof(PricePredictionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PredictPrice([FromBody] Car car)
    {
        try
        {
            var result = await _mediator.Send(new PredictPriceQuery(car));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { hata = ex.Message });
        }
    }
}