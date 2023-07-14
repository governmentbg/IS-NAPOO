using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddNewFieldsInFrameworkProgramForSection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdFrameworkProgramConnection",
                table: "SPPOO_FrameworkProgram",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdTrainingPeriod",
                table: "SPPOO_FrameworkProgram",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Practice",
                table: "SPPOO_FrameworkProgram",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SectionB",
                table: "SPPOO_FrameworkProgram",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SectionА",
                table: "SPPOO_FrameworkProgram",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SectionА1",
                table: "SPPOO_FrameworkProgram",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Theory",
                table: "SPPOO_FrameworkProgram",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdFrameworkProgramConnection",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "IdTrainingPeriod",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "Practice",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "SectionB",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "SectionА",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "SectionА1",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "Theory",
                table: "SPPOO_FrameworkProgram");
        }
    }
}
