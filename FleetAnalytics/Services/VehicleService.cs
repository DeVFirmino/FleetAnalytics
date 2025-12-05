using FleetAnalytics.DbContext;
using FleetAnalytics.DTO;
using FleetAnalytics.Interfaces;
using FleetAnalytics.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FleetAnalytics.Services;

public class VehicleService : IVehicleService
{

    private readonly FleetDbContext _context; 
    public VehicleService(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleResponseDto> AddVehicle(SaveVehicleDto request)
    {
        bool vehicleExists = await _context.Vehicles.AnyAsync(v => v.LicensePlate == request.LicensePlate);

        if (vehicleExists)
        {
            throw new Exception("Vehicle already exists with this plate number");
        }

        var newVehicle = new Vehicle
        {
            LicensePlate = request.LicensePlate,
            VehicleModel = request.VehicleModel,
            FuelCapacity = request.FuelCapacity
        };
        
         _context.Vehicles.Add(newVehicle);
         
        await _context.SaveChangesAsync();

        return new VehicleResponseDto()
        {
            Id = newVehicle.Id,
            LicensePlate = newVehicle.LicensePlate,
            VehicleModel = newVehicle.VehicleModel,
            FuelCapacity = newVehicle.FuelCapacity
        };
    }

    public async Task<List<VehicleResponseDto>> GetAllVehicles()
    {
        return await _context.Vehicles
            .Select(x => new VehicleResponseDto
        {
            Id = x.Id,
            LicensePlate = x.LicensePlate,
            VehicleModel = x.VehicleModel,
            FuelCapacity = x.FuelCapacity

        }).ToListAsync();

    }
    public async Task <VehicleResponseDto> GetById(int id)
    {
            //1 Search on DB
            var getId = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
            //2 Check if exists
            if (getId == null)
            {
                throw new KeyNotFoundException("Not found");
            }
            //3 - Mapping the Data to the Dto Response.
            var result = new VehicleResponseDto
            {
                Id = getId.Id,
                LicensePlate = getId.LicensePlate,
                FuelCapacity = getId.FuelCapacity,
                VehicleModel = getId.VehicleModel
            };
            //4 Return the Dto
            return result;
    }

    public async Task<bool> DeleteVehicle(int id)
    {
        var deleteVehicle = await _context.Vehicles.FindAsync(id);

        if (deleteVehicle == null)
        {
            return false;
        }
        _context.Vehicles.Remove(deleteVehicle);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<VehicleResponseDto> UpdateVehicle(int id, SaveVehicleDto request)
    {
        //fetch - get the existing vehicle from the db
        var vehicleUpdate = await _context.Vehicles.FindAsync(id);

        //Validate if exists or not.
        if (vehicleUpdate == null)
        {
            return null;
        }
        
        //Modify copying new dto into existing entity 
        vehicleUpdate.LicensePlate = request.LicensePlate;
        vehicleUpdate.VehicleModel = request.VehicleModel;
        vehicleUpdate.FuelCapacity = request.FuelCapacity;

        await _context.SaveChangesAsync();

        return new VehicleResponseDto()
        {
            Id = vehicleUpdate.Id,
            LicensePlate = vehicleUpdate.LicensePlate,
            VehicleModel = vehicleUpdate.VehicleModel,
            FuelCapacity = vehicleUpdate.FuelCapacity
        };

    }
}