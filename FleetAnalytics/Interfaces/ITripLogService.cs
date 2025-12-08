using FleetAnalytics.DTO;

namespace FleetAnalytics.Interfaces;

public interface ITripLogService 
{
    public Task IngestTelemetry (SaveTripLogDto request);

    public Task<List<TripLogResponseDto>> GetAllTripLogs();
    
    public Task<List<TripLogResponseDto>>  GetLogsVehicleIdById(int id);
}