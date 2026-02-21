using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetAnalytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOdometerToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LastMaintenanceOdometer",
                table: "Vehicles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Odometer",
                table: "Vehicles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMaintenanceOdometer",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Odometer",
                table: "Vehicles");
        }
    }
}
