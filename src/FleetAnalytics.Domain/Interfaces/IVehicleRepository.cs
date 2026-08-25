using FleetAnalytics.Domain.Entities;

namespace FleetAnalytics.Domain.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(int id);
    Task<List<Vehicle>> GetAllAsync();
    Task<bool> ExistsByPlateAsync(string licensePlate);
    void Add(Vehicle vehicle);
    void Update(Vehicle vehicle);
    void Delete(Vehicle vehicle);
}
