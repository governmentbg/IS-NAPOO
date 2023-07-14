using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Course_AddColumn_CourseNameEN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseNameEN",
                table: "Training_Course",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Наименование на курса на латиница");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseNameEN",
                table: "Training_Course");
        }
    }
}
