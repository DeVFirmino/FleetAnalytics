using FleetAnalytics.Application.DTOs;

namespace FleetAnalytics.Application.Interfaces;

public interface IVehicleService
{
    Task<VehicleResponseDto> AddVehicle(SaveVehicleDto request);
    Task<List<VehicleResponseDto>> GetAllVehicles();
    Task<VehicleResponseDto> GetById(int id);
    Task<bool> DeleteVehicle(int id);
    Task<VehicleResponseDto?> UpdateVehicle(int id, SaveVehicleDto request);
}
