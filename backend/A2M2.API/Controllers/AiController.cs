using A2M2.API.DTOs;
using A2M2.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace A2M2.API.Controllers;

/// <summary>
/// AI Proxy controller'ı — Fiyat tahmini (RabbitMQ + HTTP fallback)
/// </summary>
[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly RabbitMQService _rabbitMQ;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public AiController(RabbitMQService rabbitMQ, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _rabbitMQ = rabbitMQ;
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>POST /api/ai/predict-price</summary>
    [HttpPost("predict-price")]
    public async Task<IActionResult> PredictPrice([FromBody] PricePredictionRequest request)
    {
        try
        {
            // Önce RabbitMQ ile dene (RPC pattern)
            if (_rabbitMQ.IsConnected)
            {
                try
                {
                    var result = await _rabbitMQ.RequestAsync<JsonElement>(
                        RabbitMQService.PricePredictionQueue, request);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"RabbitMQ RPC hatası, HTTP fallback: {ex.Message}");
                }
            }

            // Fallback: Doğrudan HTTP ile AI servisine bağlan
            var baseUrl = _config["AiService:BaseUrl"] ?? "http://localhost:5001";
            var client = _httpClientFactory.CreateClient();

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/predict", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(body));

            return Ok(JsonSerializer.Deserialize<object>(body));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "AI servisi ile iletişim hatası", error = ex.Message });
        }
    }
}
