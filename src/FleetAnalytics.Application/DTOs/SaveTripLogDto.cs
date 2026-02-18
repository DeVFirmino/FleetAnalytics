using System.ComponentModel.DataAnnotations;

namespace FleetAnalytics.Application.DTOs;

public class SaveTripLogDto
{
    [Required]
    public int VehicleId { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Required]
    public double Speed { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }
}
