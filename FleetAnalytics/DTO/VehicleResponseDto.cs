namespace FleetAnalytics.DTO;

public class VehicleResponseDto
{
    //Response should have the id as pk to click when the response arrives to the system. 
    public int Id { get; set; }

    public string LicensePlate { get; set; }
    
    public string VehicleModel { get; set; }
    
    public double FuelCapacity { get; set; }
}
