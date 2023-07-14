using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableTraining_CourseCommissionAndAlterTableCourseRemoveColumnExamCommMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamCommMmbers",
                table: "Training_Course");

            migrationBuilder.CreateTable(
                name: "Training_CourseCommission",
                columns: table => new
                {
                    IdCourseCommission = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Име"),
                    SecondName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Презиме"),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Фамилия"),
                    IsChairman = table.Column<bool>(type: "bit", nullable: false, comment: "Дали е председател на комисия"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курса за обучение, предлаган от ЦПО"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseCommission", x => x.IdCourseCommission);
                    table.ForeignKey(
                        name: "FK_Training_CourseCommission_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Членове на изпитна комисия към курс за обучение");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseCommission_IdCourse",
                table: "Training_CourseCommission",
                column: "IdCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseCommission");

            migrationBuilder.AddColumn<string>(
                name: "ExamCommMmbers",
                table: "Training_Course",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                comment: "Членове на изпитната комисия (име и институция, която представлява)");
        }
    }
}
