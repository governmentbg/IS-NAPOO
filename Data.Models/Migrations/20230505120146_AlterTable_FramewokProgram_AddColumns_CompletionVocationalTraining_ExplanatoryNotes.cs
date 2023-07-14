using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_FramewokProgram_AddColumns_CompletionVocationalTraining_ExplanatoryNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletionVocationalTraining",
                table: "SPPOO_FrameworkProgram",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                comment: "Завършване и удостоверяване на професионалното обучение");

            migrationBuilder.AddColumn<string>(
                name: "ExplanatoryNotes",
                table: "SPPOO_FrameworkProgram",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                comment: "Пояснителни бележки");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionVocationalTraining",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "ExplanatoryNotes",
                table: "SPPOO_FrameworkProgram");
        }
    }
}
