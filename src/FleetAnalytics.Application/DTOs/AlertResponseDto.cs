namespace FleetAnalytics.Application.DTOs;

public class AlertResponseDto
{
    public int Id { get; set; }
    public string VehicleModel { get; set; } = string.Empty;
    public double Speed { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
