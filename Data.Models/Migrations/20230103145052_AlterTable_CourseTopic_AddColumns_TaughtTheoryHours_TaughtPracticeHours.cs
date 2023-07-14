using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_CourseTopic_AddColumns_TaughtTheoryHours_TaughtPracticeHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TaughtPracticeHours",
                table: "Training_CourseTopic",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                comment: "Сумарен брой преподадени часове по практика");

            migrationBuilder.AddColumn<double>(
                name: "TaughtTheoryHours",
                table: "Training_CourseTopic",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                comment: "Сумарен брой преподадени часове по теория");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaughtPracticeHours",
                table: "Training_CourseTopic");

            migrationBuilder.DropColumn(
                name: "TaughtTheoryHours",
                table: "Training_CourseTopic");
        }
    }
}
