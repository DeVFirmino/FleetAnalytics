using FleetAnalytics.Domain.Entities;

namespace FleetAnalytics.Domain.Interfaces;

public interface ITripLogRepository
{
    Task AddAsync(TripLog tripLog);
    Task<List<TripLog>> GetAllAsync();
    Task<List<TripLog>> GetByVehicleIdAsync(int vehicleId);
}
