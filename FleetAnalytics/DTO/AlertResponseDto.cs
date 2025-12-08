namespace FleetAnalytics.DTO;

public class AlertResponseDto
{
    public int Id { get; set; }
    
    public string VehicleModel { get; set; }
    
    public double Speed { get; set; }
    
    public string Type { get; set; } //convet to string to get json data
    
    public DateTime Timestamp { get; set; }
}