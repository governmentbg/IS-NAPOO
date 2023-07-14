using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class addStatusFieldTOSpecialityProfessionalDirectionProfessionArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "SPPOO_Speciality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdVQS",
                table: "SPPOO_Speciality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "SPPOO_ProfessionalDirection",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "SPPOO_Profession",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "SPPOO_Area",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "IdVQS",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "SPPOO_ProfessionalDirection");

            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "SPPOO_Profession");

            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "SPPOO_Area");
        }
    }
}
