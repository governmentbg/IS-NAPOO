using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableFrameworkProgramForColumn_MinimumLevelQualification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdMinimumQualificationLevel",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.AddColumn<string>(
                name: "MinimumLevelQualification",
                table: "SPPOO_FrameworkProgram",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumLevelQualification",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.AddColumn<int>(
                name: "IdMinimumQualificationLevel",
                table: "SPPOO_FrameworkProgram",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
