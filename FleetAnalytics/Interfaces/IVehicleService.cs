using FleetAnalytics.DTO;
using FleetAnalytics.Services;

namespace FleetAnalytics.Interfaces;

public interface IVehicleService
{

   public Task<VehicleResponseDto> AddVehicle(SaveVehicleDto request);

   public Task<List<VehicleResponseDto>> GetAllVehicles();
   //
   public Task <VehicleResponseDto> GetById(int id);
   
   public Task<bool> DeleteVehicle(int id);
   
   // Get the ID (who changes) return DTO new data.
   // update dto to return to user.
   public Task<VehicleResponseDto> UpdateVehicle(int id, SaveVehicleDto request);

}