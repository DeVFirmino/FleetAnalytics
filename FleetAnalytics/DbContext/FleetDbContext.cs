using FleetAnalytics.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetAnalytics.DbContext;

//On the db I have to create a constructor and give the parameters to pass the models. 
// I have to use the method DbSet to persist on the data, vehicle, triplogs models. 
// On model creating is where I make the Relationship between the models,
// the vehicle has many triplog, with one which means the end of of the relationship 
// has the fk of vehicle id. 
public class FleetDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<TripLog> TripLogs { get; set; }
    
    public DbSet<Alert> Alerts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vehicle>().HasMany<TripLog>()
            .WithOne()
            .HasForeignKey(log => log.VehicleId).OnDelete(DeleteBehavior.Cascade); // //Fk
    }


    
}