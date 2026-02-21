using FleetAnalytics.Application.DTOs;
using FleetAnalytics.Application.Services;
using FleetAnalytics.Domain.Entities;
using FleetAnalytics.Domain.Interfaces;
using Moq;

namespace FleetAnalytics.Tests.Services;

public class DriverServiceTests
{
    private readonly Mock<IDriverRepository> _mockRepo;
    private readonly DriverService _service;

    public DriverServiceTests()
    {
        _mockRepo = new Mock<IDriverRepository>();
        _service = new DriverService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddDriver_ShouldReturnCreatedDriver()
    {
        var request = new SaveDriverDto
        {
            FirstName = "John",
            LastName = "Doe",
            LicenseNumber = "DL-12345",
            Email = "john@example.com",
            PhoneNumber = "555-1234"
        };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Driver>()))
            .ReturnsAsync((Driver d) => { d.Id = 1; return d; });

        var result = await _service.AddDriver(request);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Driver>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnDriver_WhenExists()
    {
        var driver = new Driver { Id = 1, FirstName = "Jane", LastName = "Smith" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(driver);

        var result = await _service.GetById(1);

        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
    }

    [Fact]
    public async Task GetById_ShouldThrowKeyNotFoundException_WhenDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Driver?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetById(999));
    }
}
