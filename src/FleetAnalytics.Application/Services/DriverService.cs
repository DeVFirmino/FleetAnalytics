using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Interfaces;
using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Interfaces;

namespace FleetAnalytics.Application.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DriverService(IDriverRepository driverRepository, IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DriverResponseDto> AddDriver(SaveDriverDto request)
    {
        var newDriver = new Driver
        {
            FirstName = request.FirstName!,
            LastName = request.LastName!,
            LicenseNumber = request.LicenseNumber!,
            Email = request.Email ?? string.Empty,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            HireDate = DateTime.UtcNow,
            IsActive = true
        };

        _driverRepository.Add(newDriver);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(newDriver);
    }

    public async Task<List<DriverResponseDto>> GetAllDrivers()
    {
        var drivers = await _driverRepository.GetAllAsync();
        return drivers.Select(MapToDto).ToList();
    }

    public async Task<DriverResponseDto> GetById(int id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);

        if (driver == null)
        {
            throw new KeyNotFoundException($"Driver ID {id} not found.");
        }

        return MapToDto(driver);
    }

    public async Task<DriverResponseDto?> UpdateDriver(int id, SaveDriverDto request)
    {
        var driver = await _driverRepository.GetByIdAsync(id);

        if (driver == null)
        {
            return null;
        }

        driver.FirstName = request.FirstName!;
        driver.LastName = request.LastName!;
        driver.LicenseNumber = request.LicenseNumber!;
        driver.Email = request.Email ?? string.Empty;
        driver.PhoneNumber = request.PhoneNumber ?? string.Empty;

        _driverRepository.Update(driver);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(driver);
    }

    public async Task<bool> DeleteDriver(int id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);

        if (driver == null)
        {
            return false;
        }

        _driverRepository.Delete(driver);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static DriverResponseDto MapToDto(Driver driver)
    {
        return new DriverResponseDto
        {
            Id = driver.Id,
            FirstName = driver.FirstName,
            LastName = driver.LastName,
            LicenseNumber = driver.LicenseNumber,
            Email = driver.Email,
            PhoneNumber = driver.PhoneNumber,
            HireDate = driver.HireDate,
            IsActive = driver.IsActive
        };
    }
}
