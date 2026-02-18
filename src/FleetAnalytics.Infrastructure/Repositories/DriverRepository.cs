using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Interfaces;
using FleetAnalytics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetAnalytics.Infrastructure.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly FleetDbContext _context;

    public DriverRepository(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<Driver?> GetByIdAsync(int id)
    {
        return await _context.Drivers.FindAsync(id);
    }

    public async Task<List<Driver>> GetAllAsync()
    {
        return await _context.Drivers.ToListAsync();
    }

    public async Task<Driver> AddAsync(Driver driver)
    {
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        return driver;
    }

    public async Task UpdateAsync(Driver driver)
    {
        _context.Drivers.Update(driver);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Driver driver)
    {
        _context.Drivers.Remove(driver);
        await _context.SaveChangesAsync();
    }
}
