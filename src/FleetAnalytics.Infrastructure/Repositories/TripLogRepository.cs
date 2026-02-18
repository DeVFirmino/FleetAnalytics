using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Interfaces;
using FleetAnalytics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetAnalytics.Infrastructure.Repositories;

public class TripLogRepository : ITripLogRepository
{
    private readonly FleetDbContext _context;

    public TripLogRepository(FleetDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TripLog tripLog)
    {
        _context.TripLogs.Add(tripLog);
        await _context.SaveChangesAsync();
    }

    public async Task<List<TripLog>> GetAllAsync()
    {
        return await _context.TripLogs.ToListAsync();
    }

    public async Task<List<TripLog>> GetByVehicleIdAsync(int vehicleId)
    {
        return await _context.TripLogs
            .Where(t => t.VehicleId == vehicleId)
            .ToListAsync();
    }
}
