using FleetAnalytics.DTO;
using FleetAnalytics.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FleetAnalytics.Controllers;


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
        try
        {
            await _service.IngestTelemetry(request);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet] 
        public async Task <IActionResult> GetAllTripLogs() {
            
            var logs = await _service.GetAllTripLogs();
            
            return Ok(logs);
        }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLogsVehicleIdById(int id)
    {
        var logs = await _service.GetLogsVehicleIdById(id);

        return Ok(logs);
    }
        
    }
     
        
    



