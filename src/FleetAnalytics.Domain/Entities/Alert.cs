using FleetAnalytics.Domain.Enums;

namespace FleetAnalytics.Domain.Entities;

public class Alert
{
    public int Id { get; set; }

    public int VehicleId { get; set; }

    public AlertType Type { get; set; }

    public double Speed { get; set; }

    public DateTime Timestamp { get; set; }

    public string Details { get; set; } = string.Empty;

    // Navigation property
    public Vehicle Vehicle { get; set; } = null!;
}
