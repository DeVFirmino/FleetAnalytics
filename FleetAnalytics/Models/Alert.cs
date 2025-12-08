using FleetAnalytics.Enums;

namespace FleetAnalytics.Models;

public class Alert
{
    public int Id { get; set; }
    
    // FK that generated the alert
    public int VehicleId { get; set; } 
    
    // from Enum
    public AlertType Type { get; set; }
    
    public double Speed { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    //Info about the speed like speeding 
    public string Details { get; set; }}