namespace FleetAnalytics.Domain.Entities;

public class TripLog
{
    public int Id { get; set; }

    public int VehicleId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double Speed { get; set; }

    public DateTime Timestamp { get; set; }

    // Navigation property
    public Vehicle Vehicle { get; set; } = null!;
}
