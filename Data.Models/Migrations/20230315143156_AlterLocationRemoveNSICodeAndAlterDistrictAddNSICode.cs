using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterLocationRemoveNSICodeAndAlterDistrictAddNSICode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NSICode",
                table: "Location");

            migrationBuilder.AddColumn<int>(
                name: "NSICode",
                table: "District",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NSICode",
                table: "District");

            migrationBuilder.AddColumn<int>(
                name: "NSICode",
                table: "Location",
                type: "int",
                nullable: true);
        }
    }
}
