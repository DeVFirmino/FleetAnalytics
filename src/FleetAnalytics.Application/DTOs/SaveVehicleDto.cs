using System.ComponentModel.DataAnnotations;

namespace FleetAnalytics.Application.DTOs;

public class SaveVehicleDto
{
    [Required(ErrorMessage = "The plate is required.")]
    [StringLength(10, MinimumLength = 5, ErrorMessage = "Plate must contain 5 to 10 char")]
    public string? LicensePlate { get; set; }

    [Required(ErrorMessage = "Vehicle Model is required.")]
    public string? VehicleModel { get; set; }

    [Range(1, 1000, ErrorMessage = "Tank capacity must contain 1 to 1000.")]
    public double FuelCapacity { get; set; }
}
