using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetAnalytics.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _service;

    public VehiclesController(IVehicleService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle([FromBody] SaveVehicleDto request)
    {
        var result = await _service.AddVehicle(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehicles()
    {
        var list = await _service.GetAllVehicles();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vehicle = await _service.GetById(id);
        return Ok(vehicle);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _service.DeleteVehicle(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveVehicleDto request)
    {
        var updatedVehicle = await _service.UpdateVehicle(id, request);

        if (updatedVehicle == null)
        {
            return NotFound();
        }

        return Ok(updatedVehicle);
    }
}