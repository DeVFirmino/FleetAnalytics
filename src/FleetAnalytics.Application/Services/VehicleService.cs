using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Interfaces;
using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Interfaces;

namespace FleetAnalytics.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<VehicleResponseDto> AddVehicle(SaveVehicleDto request)
    {
        bool vehicleExists = await _vehicleRepository.ExistsByPlateAsync(request.LicensePlate!);

        if (vehicleExists)
        {
            throw new Exception("Vehicle already exists with this plate number");
        }

        var newVehicle = new Vehicle
        {
            LicensePlate = request.LicensePlate!,
            VehicleModel = request.VehicleModel!,
            FuelCapacity = request.FuelCapacity
        };

        var created = await _vehicleRepository.AddAsync(newVehicle);

        return new VehicleResponseDto
        {
            Id = created.Id,
            LicensePlate = created.LicensePlate,
            VehicleModel = created.VehicleModel,
            FuelCapacity = created.FuelCapacity
        };
    }

    public async Task<List<VehicleResponseDto>> GetAllVehicles()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();

        return vehicles.Select(v => new VehicleResponseDto
        {
            Id = v.Id,
            LicensePlate = v.LicensePlate,
            VehicleModel = v.VehicleModel,
            FuelCapacity = v.FuelCapacity
        }).ToList();
    }

    public async Task<VehicleResponseDto> GetById(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);

        if (vehicle == null)
        {
            throw new KeyNotFoundException("Not found");
        }

        return new VehicleResponseDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            FuelCapacity = vehicle.FuelCapacity,
            VehicleModel = vehicle.VehicleModel
        };
    }

    public async Task<bool> DeleteVehicle(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);

        if (vehicle == null)
        {
            return false;
        }

        await _vehicleRepository.DeleteAsync(vehicle);
        return true;
    }

    public async Task<VehicleResponseDto?> UpdateVehicle(int id, SaveVehicleDto request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);

        if (vehicle == null)
        {
            return null;
        }

        vehicle.LicensePlate = request.LicensePlate!;
        vehicle.VehicleModel = request.VehicleModel!;
        vehicle.FuelCapacity = request.FuelCapacity;

        await _vehicleRepository.UpdateAsync(vehicle);

        return new VehicleResponseDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            VehicleModel = vehicle.VehicleModel,
            FuelCapacity = vehicle.FuelCapacity
        };
    }
}
