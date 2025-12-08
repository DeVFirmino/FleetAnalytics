namespace FleetAnalytics.DTO;

public class TripLogResponseDto
{
    public string VehicleModel { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public double Speed { get; set; }
    
    public DateTime Timestamp { get; set; } //Logs

 }