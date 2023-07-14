using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableQuestion_add_IdAreaSelfAssessment_IdRatingIndicatorType_Description : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Assess_Question",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                comment: "Описание към въпроса");

            migrationBuilder.AddColumn<int>(
                name: "IdAreaSelfAssessment",
                table: "Assess_Question",
                type: "int",
                nullable: true,
                comment: "Област на самооценяване");

            migrationBuilder.AddColumn<int>(
                name: "IdRatingIndicatorType",
                table: "Assess_Question",
                type: "int",
                nullable: true,
                comment: "Вид на индикатор");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Assess_Question");

            migrationBuilder.DropColumn(
                name: "IdAreaSelfAssessment",
                table: "Assess_Question");

            migrationBuilder.DropColumn(
                name: "IdRatingIndicatorType",
                table: "Assess_Question");
        }
    }
}
