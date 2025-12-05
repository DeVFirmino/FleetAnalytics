using System.ComponentModel.DataAnnotations;

namespace FleetAnalytics.DTO;

public class SaveVehicleDto
{
    
    //using data annottations here validates tne data entry when user is saving a vehicle...
    
    [Required(ErrorMessage = "The plate is required.")]
    [StringLength(10, MinimumLength = 5, ErrorMessage = "Plate must contain 5 to 10 char")]
    public string? LicensePlate { get; set; }

    [Required(ErrorMessage = "Vehicle Model is required.")]
    public string? VehicleModel { get; set; }

    // range protects negative numbers
    [Range(1, 1000, ErrorMessage = "Tank capacity must contain 1 to 1000.")]
    public double FuelCapacity { get; set; }
}
