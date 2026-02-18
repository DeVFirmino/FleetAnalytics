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
}
