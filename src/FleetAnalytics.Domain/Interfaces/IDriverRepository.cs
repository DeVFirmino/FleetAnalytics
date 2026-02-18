using FleetAnalytics.Domain.Entities;

namespace FleetAnalytics.Domain.Interfaces;

public interface IDriverRepository
{
    Task<Driver?> GetByIdAsync(int id);
    Task<List<Driver>> GetAllAsync();
    Task<Driver> AddAsync(Driver driver);
    Task UpdateAsync(Driver driver);
    Task DeleteAsync(Driver driver);
}
