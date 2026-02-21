using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetAnalytics.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TripLogsController : ControllerBase
{
    private readonly ITripLogService _service;

    public TripLogsController(ITripLogService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> IngestTelemetry([FromBody] SaveTripLogDto request)
    {
        await _service.IngestTelemetry(request);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTripLogs()
    {
        var logs = await _service.GetAllTripLogs();
        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLogsVehicleIdById(int id)
    {
        var logs = await _service.GetLogsVehicleIdById(id);
        return Ok(logs);
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlerts()
    {
        var alerts = await _service.GetAlerts();
        return Ok(alerts);
    }
}
