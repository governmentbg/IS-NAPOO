using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesClientAndCourseClientAddFieldsIdCountryOfBirthAndIdCityOfBirth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCityOfBirth",
                table: "Training_ClientCourse",
                type: "int",
                nullable: true,
                comment: "Месторождение (населено място)");

            migrationBuilder.AddColumn<int>(
                name: "IdCountryOfBirth",
                table: "Training_ClientCourse",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Месторождение (държава)");

            migrationBuilder.AddColumn<int>(
                name: "IdCityOfBirth",
                table: "Training_Client",
                type: "int",
                nullable: true,
                comment: "Месторождение (населено място)");

            migrationBuilder.AddColumn<int>(
                name: "IdCountryOfBirth",
                table: "Training_Client",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Месторождение (държава)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCityOfBirth",
                table: "Training_ClientCourse");

            migrationBuilder.DropColumn(
                name: "IdCountryOfBirth",
                table: "Training_ClientCourse");

            migrationBuilder.DropColumn(
                name: "IdCityOfBirth",
                table: "Training_Client");

            migrationBuilder.DropColumn(
                name: "IdCountryOfBirth",
                table: "Training_Client");
        }
    }
}
