using FleetAnalytics.Domain.Entities;

namespace FleetAnalytics.Domain.Interfaces;

public interface IAlertRepository
{
    Task AddAsync(Alert alert);
    Task<List<Alert>> GetAllAsync();
}
