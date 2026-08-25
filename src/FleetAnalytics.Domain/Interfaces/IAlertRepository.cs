using FleetAnalytics.Domain.Entities;

namespace FleetAnalytics.Domain.Interfaces;

public interface IAlertRepository
{
    void Add(Alert alert);
    Task<List<Alert>> GetAllAsync();
}
