using FleetAnalytics.DbContext;
using FleetAnalytics.DTO;
using FleetAnalytics.Interfaces;
using FleetAnalytics.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace FleetAnalytics.Services;

public class TripLogService : ITripLogService
{
    private readonly FleetDbContext _context;

    public TripLogService(FleetDbContext context)
    {
        _context = context;
    }
        
    
    public async Task IngestTelemetry(SaveTripLogDto request) ///SaveTheLogDto
    {
        
      ///1 - Validates
      bool vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId);

      if (vehicleExists == null)
      {
          throw new KeyNotFoundException($"Vehicle ID {request.VehicleId} not found.");
      } 
      
      //2 - Mapping
      var newLog = new TripLog
      {
          VehicleId = request.VehicleId,
          Latitude = request.Latitude,
          Longitude = request.Longitude,
          Timestamp = request.Timestamp
      };

      //3 - Persists.
      _context.TripLogs.Add(newLog);

      await _context.SaveChangesAsync();

    }

    public async Task<List<TripLogResponseDto>> GetAllTripLogs()
    {
        //Go to the TripLogstable
        return await _context.TripLogs
            .Select(log => new TripLogResponseDto
            { //Get the property
                Latitude = log.Latitude,
                Longitude = log.Longitude,
                Speed = log.Speed,
                Timestamp = log.Timestamp,
                
                //Mapping the property using the join and
                //subquerie to find the behicle model on the table
                //using the id where the table is
                VehicleModel = _context.Vehicles
                    .Where(v => v.Id == log.VehicleId)
                    .Select(v => v.VehicleModel)
                    .FirstOrDefault() })
                    .ToListAsync();
    }

    public async Task<List<TripLogResponseDto>> GetLogsVehicleIdById(int id)
    {
        return await _context.TripLogs.Where(log => log.VehicleId == id).Select(log => new TripLogResponseDto
            { //Get the property
                Latitude = log.Latitude,
                Longitude = log.Longitude,
                Speed = log.Speed,
                Timestamp = log.Timestamp,
                
                //Mapping the property using the join and
                //subquerie to find the behicle model on the table
                //using the id where the table is
                VehicleModel = _context.Vehicles
                    .Where(v => v.Id == log.VehicleId)
                    .Select(v => v.VehicleModel)
                    .FirstOrDefault() })
            .ToListAsync();
    }
}