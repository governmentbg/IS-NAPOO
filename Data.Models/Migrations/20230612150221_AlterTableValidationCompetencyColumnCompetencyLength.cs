using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableValidationCompetencyColumnCompetencyLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Competency",
                table: "Training_ValidationCompetency",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "Компетентност",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "Компетентност");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Competency",
                table: "Training_ValidationCompetency",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "Компетентност",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldComment: "Компетентност");
        }
    }
}
