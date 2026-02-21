namespace FleetAnalytics.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }

    public string LicensePlate { get; set; } = string.Empty;

    public string VehicleModel { get; set; } = string.Empty;

    public double FuelCapacity { get; set; }
    public double Odometer { get; set; }
    public double LastMaintenanceOdometer { get; set; }

    // Navigation properties
    public ICollection<TripLog> TripLogs { get; set; } = new List<TripLog>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
