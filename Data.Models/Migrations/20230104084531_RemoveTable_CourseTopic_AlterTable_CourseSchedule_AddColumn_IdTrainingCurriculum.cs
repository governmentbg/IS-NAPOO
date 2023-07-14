using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class RemoveTable_CourseTopic_AlterTable_CourseSchedule_AddColumn_IdTrainingCurriculum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_CourseSchedule_Training_CourseTopic_IdCourseTopic",
                table: "Training_CourseSchedule");

            migrationBuilder.DropTable(
                name: "Training_CourseTopic");

            migrationBuilder.RenameColumn(
                name: "IdCourseTopic",
                table: "Training_CourseSchedule",
                newName: "IdTrainingCurriculum");

            migrationBuilder.RenameIndex(
                name: "IX_Training_CourseSchedule_IdCourseTopic",
                table: "Training_CourseSchedule",
                newName: "IX_Training_CourseSchedule_IdTrainingCurriculum");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_CourseSchedule_Training_Curriculum_IdTrainingCurriculum",
                table: "Training_CourseSchedule",
                column: "IdTrainingCurriculum",
                principalTable: "Training_Curriculum",
                principalColumn: "IdTrainingCurriculum",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_CourseSchedule_Training_Curriculum_IdTrainingCurriculum",
                table: "Training_CourseSchedule");

            migrationBuilder.RenameColumn(
                name: "IdTrainingCurriculum",
                table: "Training_CourseSchedule",
                newName: "IdCourseTopic");

            migrationBuilder.RenameIndex(
                name: "IX_Training_CourseSchedule_IdTrainingCurriculum",
                table: "Training_CourseSchedule",
                newName: "IX_Training_CourseSchedule_IdCourseTopic");

            migrationBuilder.CreateTable(
                name: "Training_CourseTopic",
                columns: table => new
                {
                    IdCourseTopic = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с курс за обучение, предлагани от ЦПО"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PracticeHours = table.Column<double>(type: "float", nullable: false, comment: "Сумарен брой часове по практика"),
                    TaughtPracticeHours = table.Column<double>(type: "float", nullable: false, comment: "Сумарен брой преподадени часове по практика"),
                    TaughtTheoryHours = table.Column<double>(type: "float", nullable: false, comment: "Сумарен брой преподадени часове по теория"),
                    TheoryHours = table.Column<double>(type: "float", nullable: false, comment: "Сумарен брой часове по теория"),
                    Topic = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseTopic", x => x.IdCourseTopic);
                    table.ForeignKey(
                        name: "FK_Training_CourseTopic_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseTopic_IdCourse",
                table: "Training_CourseTopic",
                column: "IdCourse");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_CourseSchedule_Training_CourseTopic_IdCourseTopic",
                table: "Training_CourseSchedule",
                column: "IdCourseTopic",
                principalTable: "Training_CourseTopic",
                principalColumn: "IdCourseTopic",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
