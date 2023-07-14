using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_AnnualInfo_add_HasCoursePerYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasCoursePerYear",
                table: "Arch_AnnualInfo",
                type: "bit",
                nullable: true,
                comment: "Оказва дали има приключили курсове през годината");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasCoursePerYear",
                table: "Arch_AnnualInfo");
        }
    }
}
