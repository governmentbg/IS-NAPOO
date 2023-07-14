using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Survey_AddColumn_IdSurveyStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Procedure_Payment",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5,
                oldComment: "Валута в която се плаща задължението (три символа, пр. \"BGN\")");

            migrationBuilder.AddColumn<int>(
                name: "IdSurveyStatus",
                table: "Assess_Survey",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Статус на анкетата");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdSurveyStatus",
                table: "Assess_Survey");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Procedure_Payment",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                comment: "Валута в която се плаща задължението (три символа, пр. \"BGN\")",
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);
        }
    }
}
