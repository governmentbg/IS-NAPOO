using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddAdditionalfieldsInSpecialityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdultEducation",
                table: "SPPOO_Speciality",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDistanceLearning",
                table: "SPPOO_Speciality",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShortageSpecialistsLaborMarket",
                table: "SPPOO_Speciality",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStateProtectedSpecialties",
                table: "SPPOO_Speciality",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrainingPartProfession",
                table: "SPPOO_Speciality",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrainingStudents",
                table: "SPPOO_Speciality",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdultEducation",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "IsDistanceLearning",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "IsShortageSpecialistsLaborMarket",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "IsStateProtectedSpecialties",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "IsTrainingPartProfession",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "IsTrainingStudents",
                table: "SPPOO_Speciality");
        }
    }
}
