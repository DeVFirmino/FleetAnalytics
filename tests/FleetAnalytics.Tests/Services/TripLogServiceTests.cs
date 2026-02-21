using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Services;
using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Enums;
using FleetAnalytics.Domain.Interfaces;
using Moq;

namespace FleetAnalytics.Tests.Services;

public class TripLogServiceTests
{
    private readonly Mock<ITripLogRepository> _mockTripLogRepo;
    private readonly Mock<IVehicleRepository> _mockVehicleRepo;
    private readonly Mock<IAlertRepository> _mockAlertRepo;
    private readonly TripLogService _service;

    public TripLogServiceTests()
    {
        _mockTripLogRepo = new Mock<ITripLogRepository>();
        _mockVehicleRepo = new Mock<IVehicleRepository>();
        _mockAlertRepo = new Mock<IAlertRepository>();

        _service = new TripLogService(
            _mockTripLogRepo.Object,
            _mockVehicleRepo.Object,
            _mockAlertRepo.Object);
    }

    [Fact]
    public async Task IngestTelemetry_ShouldThrowException_WhenVehicleNotFound()
    {
        var request = new SaveTripLogDto { VehicleId = 999 };
        _mockVehicleRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Vehicle?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.IngestTelemetry(request));
    }

    [Fact]
    public async Task IngestTelemetry_ShouldCreateHighSpeedAlert_WhenSpeedExceedsLimit()
    {
        var vehicle = new Vehicle { Id = 1, Odometer = 1000, LastMaintenanceOdometer = 1000 };
        var request = new SaveTripLogDto { VehicleId = 1, Speed = 90.0, Timestamp = DateTime.UtcNow }; // Speed > 80

        _mockVehicleRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);

        await _service.IngestTelemetry(request);

        _mockAlertRepo.Verify(r => r.AddAsync(It.Is<Alert>(a => a.Type == AlertType.HighSpeed)), Times.Once);
        _mockTripLogRepo.Verify(r => r.AddAsync(It.IsAny<TripLog>()), Times.Once);
    }

    [Fact]
    public async Task IngestTelemetry_ShouldCreateMaintenanceAlert_WhenOdometerExceedsThreshold()
    {
        var vehicle = new Vehicle { Id = 1, Odometer = 9995, LastMaintenanceOdometer = 0 };
        // Old log is slightly "away". We move a few km to tick it over 10000.
        // Approx 1 degree latitude = 111 km. 0.1 degree = 11.1 km.
        var lastLog = new TripLog { Latitude = 40.0, Longitude = 40.0 };
        var request = new SaveTripLogDto { VehicleId = 1, Latitude = 40.1, Longitude = 40.0, Speed = 50.0 };

        _mockVehicleRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);
        _mockTripLogRepo.Setup(r => r.GetLatestByVehicleIdAsync(1)).ReturnsAsync(lastLog);

        await _service.IngestTelemetry(request);

        // Odometer should have increased by ~11.1km, total is now > 10000.
        _mockAlertRepo.Verify(r => r.AddAsync(It.Is<Alert>(a => a.Type == AlertType.MaintenanceDue)), Times.Once);
        _mockVehicleRepo.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>()), Times.Once);
    }
}
