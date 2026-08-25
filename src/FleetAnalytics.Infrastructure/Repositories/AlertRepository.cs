using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Interfaces;
using FleetAnalytics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetAnalytics.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly FleetDbContext _context;

    public AlertRepository(FleetDbContext context)
    {
        _context = context;
    }

    public void Add(Alert alert)
    {
        _context.Alerts.Add(alert);
    }

    public async Task<List<Alert>> GetAllAsync()
    {
        return await _context.Alerts.ToListAsync();
    }
}
