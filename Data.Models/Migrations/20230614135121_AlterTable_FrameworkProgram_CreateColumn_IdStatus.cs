using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_FrameworkProgram_CreateColumn_IdStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "SPPOO_FrameworkProgram",
                type: "int",
                nullable: true,
                comment: "Статус на рамкова програма");

            migrationBuilder.AlterColumn<string>(
                name: "NegativeIssueText",
                table: "Procedure_NegativeIssue",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.AlterColumn<string>(
                name: "NegativeIssueText",
                table: "Procedure_NegativeIssue",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);
        }
    }
}
