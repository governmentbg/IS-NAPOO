using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterValidationClientsIndentLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Indent",
                table: "Training_ValidationClient",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "ЕГН/ЛНЧ/ИДН",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "ЕГН/ЛНЧ/ИДН");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Indent",
                table: "Training_ValidationClient",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                comment: "ЕГН/ЛНЧ/ИДН",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "ЕГН/ЛНЧ/ИДН");
        }
    }
}
