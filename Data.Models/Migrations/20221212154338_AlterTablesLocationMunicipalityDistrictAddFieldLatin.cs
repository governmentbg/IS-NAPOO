using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesLocationMunicipalityDistrictAddFieldLatin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MunicipalityNameEN",
                table: "Municipality",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocationNameEN",
                table: "Location",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DistrictNameEN",
                table: "District",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MunicipalityNameEN",
                table: "Municipality");

            migrationBuilder.DropColumn(
                name: "LocationNameEN",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "DistrictNameEN",
                table: "District");
        }
    }
}
