using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Services;
using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Enums;
using FleetAnalytics.Domain.Interfaces;
using FleetAnalytics.Infrastructure.Data;
using FleetAnalytics.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FleetAnalytics.Tests.Services;

/// <summary>
/// Runs the ingestion flow against a real SQLite database to prove the
/// atomicity guarantee: a trip log and its alerts are committed together,
/// and a failure mid-operation leaves nothing behind — no orphan alerts.
/// </summary>
public sealed class TripLogIngestionAtomicityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<FleetDbContext> _options;

    public TripLogIngestionAtomicityTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<FleetDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new FleetDbContext(_options);
        context.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task IngestTelemetry_ShouldPersistLogAndAlertTogether_WhenSpeedExceedsLimit()
    {
        var vehicleId = await SeedVehicleAsync();

        await using var context = new FleetDbContext(_options);
        var service = new TripLogService(
            new TripLogRepository(context),
            new VehicleRepository(context),
            new AlertRepository(context),
            context);

        await service.IngestTelemetry(new SaveTripLogDto
        {
            VehicleId = vehicleId,
            Latitude = 35.9,
            Longitude = 14.5,
            Speed = 125.0,
            Timestamp = DateTime.UtcNow
        });

        await using var verification = new FleetDbContext(_options);
        Assert.Equal(1, await verification.TripLogs.CountAsync());
        var alert = Assert.Single(await verification.Alerts.ToListAsync());
        Assert.Equal(AlertType.HighSpeed, alert.Type);
    }

    [Fact]
    public async Task IngestTelemetry_ShouldPersistNothing_WhenFailureOccursAfterAlertIsStaged()
    {
        var vehicleId = await SeedVehicleAsync();

        await using var context = new FleetDbContext(_options);

        // The speeding alert is staged first; the trip-log step then fails.
        var failingTripLogRepo = new Mock<ITripLogRepository>();
        failingTripLogRepo
            .Setup(r => r.Add(It.IsAny<TripLog>()))
            .Throws(new InvalidOperationException("Simulated failure after the alert was staged"));

        var service = new TripLogService(
            failingTripLogRepo.Object,
            new VehicleRepository(context),
            new AlertRepository(context),
            context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.IngestTelemetry(new SaveTripLogDto
        {
            VehicleId = vehicleId,
            Latitude = 35.9,
            Longitude = 14.5,
            Speed = 125.0,
            Timestamp = DateTime.UtcNow
        }));

        // Nothing was committed: no orphan alert, no trip log.
        await using var verification = new FleetDbContext(_options);
        Assert.Equal(0, await verification.Alerts.CountAsync());
        Assert.Equal(0, await verification.TripLogs.CountAsync());
    }

    private async Task<int> SeedVehicleAsync()
    {
        await using var context = new FleetDbContext(_options);
        var vehicle = new Vehicle { LicensePlate = "ABC-1234", VehicleModel = "Volvo", FuelCapacity = 60 };
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();
        return vehicle.Id;
    }
}
