using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCourseColumnAdditionalInfoType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdditionalNotes",
                table: "Training_Course",
                type: "varchar(MAX)",
                nullable: true,
                comment: "Други пояснения",
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true,
                oldComment: "Други пояснения");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdditionalNotes",
                table: "Training_Course",
                type: "ntext",
                nullable: true,
                comment: "Други пояснения",
                oldClrType: typeof(string),
                oldType: "varchar(MAX)",
                oldNullable: true,
                oldComment: "Други пояснения");
        }
    }
}
