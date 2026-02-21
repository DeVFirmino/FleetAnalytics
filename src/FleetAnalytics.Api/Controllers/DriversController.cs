using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetAnalytics.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly IDriverService _service;

    public DriversController(IDriverService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddDriver([FromBody] SaveDriverDto request)
    {
        var result = await _service.AddDriver(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDrivers()
    {
        var drivers = await _service.GetAllDrivers();
        return Ok(drivers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var driver = await _service.GetById(id);
        return Ok(driver);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveDriverDto request)
    {
        var updatedDriver = await _service.UpdateDriver(id, request);

        if (updatedDriver == null)
        {
            return NotFound();
        }

        return Ok(updatedDriver);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _service.DeleteDriver(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
