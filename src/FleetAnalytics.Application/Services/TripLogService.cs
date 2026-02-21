using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Interfaces;
using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Enums;
using FleetAnalytics.Domain.Interfaces;

namespace FleetAnalytics.Application.Services;

public class TripLogService : ITripLogService
{
    private readonly ITripLogRepository _tripLogRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAlertRepository _alertRepository;

    public TripLogService(
        ITripLogRepository tripLogRepository,
        IVehicleRepository vehicleRepository,
        IAlertRepository alertRepository)
    {
        _tripLogRepository = tripLogRepository;
        _vehicleRepository = vehicleRepository;
        _alertRepository = alertRepository;
    }

    public async Task IngestTelemetry(SaveTripLogDto request)
    {
        const double SPEED_LIMIT = 80.0;

        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId);

        if (vehicle == null)
        {
            throw new KeyNotFoundException($"Vehicle ID {request.VehicleId} not found.");
        }

        if (request.Speed > SPEED_LIMIT)
        {
            var speedingAlert = new Alert
            {
                VehicleId = request.VehicleId,
                Type = AlertType.HighSpeed,
                Timestamp = request.Timestamp,
                Speed = request.Speed,
                Details = $"Exceeded speed limit of {SPEED_LIMIT} km/h. Recorded speed: {request.Speed} km/h"
            };
            await _alertRepository.AddAsync(speedingAlert);
        }

        var lastLog = await _tripLogRepository.GetLatestByVehicleIdAsync(request.VehicleId);
        if (lastLog != null)
        {
            var distance = CalculateDistance(lastLog.Latitude, lastLog.Longitude, request.Latitude, request.Longitude);
            vehicle.Odometer += distance;

            // Trigger maintenance alert if distance since last maintenance > 10000 km
            if (vehicle.Odometer - vehicle.LastMaintenanceOdometer > 10000)
            {
                var maintenanceAlert = new Alert
                {
                    VehicleId = vehicle.Id,
                    Type = AlertType.MaintenanceDue,
                    Timestamp = request.Timestamp,
                    Speed = request.Speed,
                    Details = $"Maintenance due. Odometer: {Math.Round(vehicle.Odometer, 2)} km. Last maintenance: {Math.Round(vehicle.LastMaintenanceOdometer, 2)} km."
                };
                await _alertRepository.AddAsync(maintenanceAlert);
            }

            await _vehicleRepository.UpdateAsync(vehicle);
        }

        var newLog = new TripLog
        {
            VehicleId = request.VehicleId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Timestamp = request.Timestamp,
            Speed = request.Speed
        };

        await _tripLogRepository.AddAsync(newLog);
    }

    public async Task<List<TripLogResponseDto>> GetAllTripLogs()
    {
        var logs = await _tripLogRepository.GetAllAsync();
        var vehicles = await _vehicleRepository.GetAllAsync();
        var vehicleLookup = vehicles.ToDictionary(v => v.Id, v => v.VehicleModel);

        return logs.Select(log => new TripLogResponseDto
        {
            Latitude = log.Latitude,
            Longitude = log.Longitude,
            Speed = log.Speed,
            Timestamp = log.Timestamp,
            VehicleModel = vehicleLookup.GetValueOrDefault(log.VehicleId, "Unknown")
        }).ToList();
    }

    public async Task<List<TripLogResponseDto>> GetLogsVehicleIdById(int id)
    {
        var logs = await _tripLogRepository.GetByVehicleIdAsync(id);
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        var vehicleModel = vehicle?.VehicleModel ?? "Unknown";

        return logs.Select(log => new TripLogResponseDto
        {
            Latitude = log.Latitude,
            Longitude = log.Longitude,
            Speed = log.Speed,
            Timestamp = log.Timestamp,
            VehicleModel = vehicleModel
        }).ToList();
    }

    public async Task<List<AlertResponseDto>> GetAlerts()
    {
        var alerts = await _alertRepository.GetAllAsync();
        var vehicles = await _vehicleRepository.GetAllAsync();
        var vehicleLookup = vehicles.ToDictionary(v => v.Id, v => v.VehicleModel);

        return alerts.Select(a => new AlertResponseDto
        {
            Id = a.Id,
            Timestamp = a.Timestamp,
            Speed = a.Speed,
            Type = a.Type.ToString(),
            VehicleModel = vehicleLookup.GetValueOrDefault(a.VehicleId, "Unknown")
        }).ToList();
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Radius of the earth in km
        var dLat = Deg2Rad(lat2 - lat1);
        var dLon = Deg2Rad(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance in km
    }

    private static double Deg2Rad(double deg)
    {
        return deg * (Math.PI / 180);
    }
}
