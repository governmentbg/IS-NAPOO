using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProgramAlterTypeColumn_ProgramNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProgramNote",
                table: "Training_Program",
                type: "ntext",
                nullable: true,
                comment: "Допълнителна информация",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true,
                oldComment: "Допълнителна информация");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProgramNote",
                table: "Training_Program",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                comment: "Допълнителна информация",
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true,
                oldComment: "Допълнителна информация");
        }
    }
}
