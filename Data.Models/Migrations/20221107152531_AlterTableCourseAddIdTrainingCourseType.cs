using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCourseAddIdTrainingCourseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdTrainingCourseType",
                table: "Training_Course",
                type: "int",
                nullable: true,
                comment: "Вид на курса за обучение");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdTrainingCourseType",
                table: "Training_Course");
        }
    }
}
