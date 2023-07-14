using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCourse_AdditionalNotes_NTEXT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdditionalNotes",
                table: "Training_Course",
                type: "ntext",
                nullable: true,
                comment: "Други пояснения",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true,
                oldComment: "Други пояснения");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdditionalNotes",
                table: "Training_Course",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                comment: "Други пояснения",
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true,
                oldComment: "Други пояснения");
        }
    }
}
