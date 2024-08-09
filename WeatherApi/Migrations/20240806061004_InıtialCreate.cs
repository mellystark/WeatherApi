using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherApi.Migrations
{
    /// <inheritdoc />
    public partial class InıtialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    WindSpeed = table.Column<int>(type: "int", nullable: false),
                    WindDirection = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherConditions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherConditions");
        }
    }
}
