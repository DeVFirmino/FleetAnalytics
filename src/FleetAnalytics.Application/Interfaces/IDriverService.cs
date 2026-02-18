using FleetAnalytics.Application.DTOs;

namespace FleetAnalytics.Application.Interfaces;

public interface IDriverService
{
    Task<DriverResponseDto> AddDriver(SaveDriverDto request);
    Task<List<DriverResponseDto>> GetAllDrivers();
    Task<DriverResponseDto> GetById(int id);
    Task<DriverResponseDto?> UpdateDriver(int id, SaveDriverDto request);
    Task<bool> DeleteDriver(int id);
}
