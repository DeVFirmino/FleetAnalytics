using FleetAnalytics.Domain.Entities;

namespace FleetAnalytics.Domain.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(int id);
    Task<List<Vehicle>> GetAllAsync();
    Task<bool> ExistsByPlateAsync(string licensePlate);
    Task<Vehicle> AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);
}
