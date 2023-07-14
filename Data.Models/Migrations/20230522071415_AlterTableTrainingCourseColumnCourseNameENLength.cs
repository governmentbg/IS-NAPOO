using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTrainingCourseColumnCourseNameENLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CourseNameEN",
                table: "Training_Course",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Наименование на курса на латиница",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Наименование на курса на латиница");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CourseNameEN",
                table: "Training_Course",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Наименование на курса на латиница",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComment: "Наименование на курса на латиница");
        }
    }
}
