using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Interfaces;
using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Interfaces;

namespace FleetAnalytics.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VehicleService(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
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
            FuelCapacity = request.FuelCapacity,
            Odometer = request.Odometer,
            LastMaintenanceOdometer = request.Odometer // Initialize to current
        };

        _vehicleRepository.Add(newVehicle);
        await _unitOfWork.SaveChangesAsync();

        return new VehicleResponseDto
        {
            Id = newVehicle.Id,
            LicensePlate = newVehicle.LicensePlate,
            VehicleModel = newVehicle.VehicleModel,
            FuelCapacity = newVehicle.FuelCapacity,
            Odometer = newVehicle.Odometer
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
            FuelCapacity = v.FuelCapacity,
            Odometer = v.Odometer
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
            VehicleModel = vehicle.VehicleModel,
            Odometer = vehicle.Odometer
        };
    }

    public async Task<bool> DeleteVehicle(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);

        if (vehicle == null)
        {
            return false;
        }

        _vehicleRepository.Delete(vehicle);
        await _unitOfWork.SaveChangesAsync();
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
        vehicle.Odometer = request.Odometer;

        _vehicleRepository.Update(vehicle);
        await _unitOfWork.SaveChangesAsync();

        return new VehicleResponseDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            VehicleModel = vehicle.VehicleModel,
            FuelCapacity = vehicle.FuelCapacity,
            Odometer = vehicle.Odometer
        };
    }
}
