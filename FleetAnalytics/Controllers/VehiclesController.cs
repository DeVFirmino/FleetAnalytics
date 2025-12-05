using Microsoft.AspNetCore.Mvc;
using FleetAnalytics.DbContext;
using FleetAnalytics.Models;
using SQLitePCL;

namespace FleetAnalytics.Controllers;

[ApiController]
[Route("api/[controller]")]


public class VehiclesController : ControllerBase
{

    private readonly FleetDbContext _context;

    public VehiclesController(FleetDbContext context)
    {
        _context = context;
    }


    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle([FromBody] Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);

        await _context.SaveChangesAsync();
        return Ok(vehicle);
    }
    
    
    
    



}