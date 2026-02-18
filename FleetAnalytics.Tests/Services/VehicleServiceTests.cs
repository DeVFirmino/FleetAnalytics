using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using FleetAnalytics.Services;
using FleetAnalytics.DTO;
using FleetAnalytics.DbContext;
using FleetAnalytics.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FleetAnalytics.Tests.Services;

public class VehicleServiceTests
{
    private readonly FleetDbContext _context;
    private readonly VehicleService _vehicleService;

    public VehicleServiceTests()
    {
        var options = new DbContextOptionsBuilder<FleetDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        _context = new FleetDbContext(options);
        _vehicleService = new VehicleService(_context);
    }

    [Fact]
    public async Task AddVehicle_ShouldAddVehicle_WhenVehicleDoesNotExist()
    {
        // Arrange
        var request = new SaveVehicleDto
        {
            LicensePlate = "ABC-1234",
            VehicleModel = "Toyota Prius",
            FuelCapacity = 45
        };

        // Act
        var result = await _vehicleService.AddVehicle(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.LicensePlate, result.LicensePlate);
        Assert.Equal(request.VehicleModel, result.VehicleModel);
        Assert.Equal(request.FuelCapacity, result.FuelCapacity);

        var vehicleInDb = await _context.Vehicles.FirstOrDefaultAsync(v => v.LicensePlate == request.LicensePlate);
        Assert.NotNull(vehicleInDb);
    }

    [Fact]
    public async Task AddVehicle_ShouldThrowException_WhenVehicleExists()
    {
        // Arrange
        var existingVehicle = new Vehicle
        {
            LicensePlate = "ABC-1234",
            VehicleModel = "Honda Civic",
            FuelCapacity = 50
        };
        _context.Vehicles.Add(existingVehicle);
        await _context.SaveChangesAsync();

        var request = new SaveVehicleDto
        {
            LicensePlate = "ABC-1234", // Duplicate license plate
            VehicleModel = "Toyota Prius",
            FuelCapacity = 45
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _vehicleService.AddVehicle(request));
    }

    [Fact]
    public async Task GetById_ShouldReturnVehicle_WhenVehicleExists()
    {
        // Arrange
        var vehicle = new Vehicle
        {
            LicensePlate = "XYZ-9876",
            VehicleModel = "Ford Fusion",
            FuelCapacity = 60
        };
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        // Act
        var result = await _vehicleService.GetById(vehicle.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.Id);
        Assert.Equal(vehicle.LicensePlate, result.LicensePlate);
    }

    [Fact]
    public async Task GetById_ShouldThrowKeyNotFoundException_WhenVehicleDoesNotExist()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _vehicleService.GetById(999));
    }
}
