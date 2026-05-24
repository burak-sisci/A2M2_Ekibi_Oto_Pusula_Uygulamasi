using backend.API.Modules.Auth.Application;
using backend.API.Modules.Auth.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.API.Presentation.Controllers;

[ApiController]
[Route("devices")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceRepository _devices;

    public DevicesController(IDeviceRepository devices)
    {
        _devices = devices;
    }

    /// <summary>POST /devices/register — Mobil cihazın FCM token'ını kaydeder (upsert).</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DeviceRegisterRequest istek)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(istek.FcmToken))
            return BadRequest(new { mesaj = "fcmToken boş olamaz." });

        await _devices.UpsertAsync(new Device
        {
            UserId   = userId,
            FcmToken = istek.FcmToken,
            Platform = string.IsNullOrEmpty(istek.Platform) ? "android" : istek.Platform
        });

        return Ok(new { mesaj = "Cihaz kaydedildi." });
    }

    /// <summary>DELETE /devices/unregister — Token'ı sil (logout veya cihaz reset'i).</summary>
    [HttpDelete("unregister")]
    public async Task<IActionResult> Unregister([FromBody] DeviceUnregisterRequest istek)
    {
        if (string.IsNullOrWhiteSpace(istek.FcmToken))
            return BadRequest(new { mesaj = "fcmToken boş olamaz." });

        await _devices.DeleteByTokenAsync(istek.FcmToken);
        return Ok(new { mesaj = "Cihaz silindi." });
    }
}

public record DeviceRegisterRequest(string FcmToken, string? Platform = "android");
public record DeviceUnregisterRequest(string FcmToken);
