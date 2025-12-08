using FleetAnalytics.DbContext;
using FleetAnalytics.DTO;
using FleetAnalytics.Enums;
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
        //Created a constant of speedlimit where I make the 80km default
        const double SPEED_LIMIT = 80.0;  
        
      //1 - Validate if the vehicle exists using bool.
      bool vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId); 
 
      //Createed a if clause of if is not true I throw an error that vehicle not exists.
      if (!vehicleExists)
      {
          throw new KeyNotFoundException($"Vehicle ID {request.VehicleId} not found.");
      }
        //I created another clause of if the Speed saved on the DTO is more than
        //the Speed_Limit constant of 80 I create an alert
      if (request.Speed > SPEED_LIMIT) 
      {
          //I create a new alert object from my model alert and add it to the DTO. 
          var speedingAlert = new Alert //Instance a new object of alert and passes the log to the DTO
          {
              VehicleId = request.VehicleId,
              Type = AlertType.HighSpeed,
              Timestamp = request.Timestamp,
              Speed = request.Speed,
              Details = $"Exceeded speed limit of {SPEED_LIMIT} km/h. Recorded speed: {request.Speed} km"
          };
          _context.Alerts.Add(speedingAlert);
      }
      
      //I go to the db, alerts model and add my new object of alert that I instanced as speedAlert.
       
       
      //2 - Mapping 
      //Create a new object of TripLog and add the DTO to it.
      var newLog = new TripLog
      {
          VehicleId = request.VehicleId,
          Latitude = request.Latitude,
          Longitude = request.Longitude,
          Timestamp = request.Timestamp,
          Speed = request.Speed
      };
      
      
      //Persists the triplogs model on the dto and add mapped the new log to the triplogs
      _context.TripLogs.Add(newLog);
        
      //Async method to persists on the DB. 
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

    public async Task<List<AlertResponseDto>> GetAlerts()
    {
         return await _context.Alerts.Select(x => new AlertResponseDto
         {
             Id = x.Id,
             Timestamp = x.Timestamp,
             Speed = x.Speed,
             Type = x.Type.ToString(),
             VehicleModel = _context.Vehicles.Where(v => v.Id == x.VehicleId).
                 Select(v => v.VehicleModel).FirstOrDefault()
             
         }).ToListAsync();
    }
}