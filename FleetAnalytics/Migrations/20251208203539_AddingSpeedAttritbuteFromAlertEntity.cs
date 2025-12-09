using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetAnalytics.Migrations
{
    /// <inheritdoc />
    public partial class AddingSpeedAttritbuteFromAlertEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Speed",
                table: "Alerts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Speed",
                table: "Alerts");
        }
    }
}
