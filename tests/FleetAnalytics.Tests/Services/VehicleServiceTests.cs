using Xunit;
using Moq;
using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Services;
using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Interfaces;

namespace FleetAnalytics.Tests.Services;

public class VehicleServiceTests
{
    private readonly Mock<IVehicleRepository> _mockRepo;
    private readonly VehicleService _vehicleService;

    public VehicleServiceTests()
    {
        _mockRepo = new Mock<IVehicleRepository>();
        _vehicleService = new VehicleService(_mockRepo.Object);
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

        _mockRepo.Setup(r => r.ExistsByPlateAsync("ABC-1234")).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Vehicle>()))
            .ReturnsAsync((Vehicle v) => { v.Id = 1; return v; });

        // Act
        var result = await _vehicleService.AddVehicle(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.LicensePlate, result.LicensePlate);
        Assert.Equal(request.VehicleModel, result.VehicleModel);
        Assert.Equal(request.FuelCapacity, result.FuelCapacity);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
    }

    [Fact]
    public async Task AddVehicle_ShouldThrowException_WhenVehicleExists()
    {
        // Arrange
        _mockRepo.Setup(r => r.ExistsByPlateAsync("ABC-1234")).ReturnsAsync(true);

        var request = new SaveVehicleDto
        {
            LicensePlate = "ABC-1234",
            VehicleModel = "Toyota Prius",
            FuelCapacity = 45
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _vehicleService.AddVehicle(request));
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Never);
    }

    [Fact]
    public async Task GetById_ShouldReturnVehicle_WhenVehicleExists()
    {
        // Arrange
        var vehicle = new Vehicle
        {
            Id = 1,
            LicensePlate = "XYZ-9876",
            VehicleModel = "Ford Fusion",
            FuelCapacity = 60
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);

        // Act
        var result = await _vehicleService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.Id);
        Assert.Equal(vehicle.LicensePlate, result.LicensePlate);
    }

    [Fact]
    public async Task GetById_ShouldThrowKeyNotFoundException_WhenVehicleDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Vehicle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _vehicleService.GetById(999));
    }

    [Fact]
    public async Task DeleteVehicle_ShouldReturnTrue_WhenVehicleExists()
    {
        // Arrange
        var vehicle = new Vehicle { Id = 1, LicensePlate = "ABC-1234", VehicleModel = "Test", FuelCapacity = 50 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);

        // Act
        var result = await _vehicleService.DeleteVehicle(1);

        // Assert
        Assert.True(result);
        _mockRepo.Verify(r => r.DeleteAsync(vehicle), Times.Once);
    }

    [Fact]
    public async Task DeleteVehicle_ShouldReturnFalse_WhenVehicleDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Vehicle?)null);

        // Act
        var result = await _vehicleService.DeleteVehicle(999);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Vehicle>()), Times.Never);
    }
}
