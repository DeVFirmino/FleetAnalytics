using FleetAnalytics.Application.DTOs;

namespace FleetAnalytics.Application.Interfaces;

public interface ITripLogService
{
    Task IngestTelemetry(SaveTripLogDto request);
    Task<List<TripLogResponseDto>> GetAllTripLogs();
    Task<List<TripLogResponseDto>> GetLogsVehicleIdById(int id);
    Task<List<AlertResponseDto>> GetAlerts();
}
