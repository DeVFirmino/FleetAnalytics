namespace FleetAnalytics.Application.DTOs;

public class VehicleResponseDto
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public double FuelCapacity { get; set; }
}
