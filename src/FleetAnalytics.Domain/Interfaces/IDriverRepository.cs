using FleetAnalytics.Domain.Entities;

namespace FleetAnalytics.Domain.Interfaces;

public interface IDriverRepository
{
    Task<Driver?> GetByIdAsync(int id);
    Task<List<Driver>> GetAllAsync();
    void Add(Driver driver);
    void Update(Driver driver);
    void Delete(Driver driver);
}
