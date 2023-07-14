using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_CourseCandidateCurriculumModification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_CourseCandidateCurriculumModification_Training_Course_CourseIdCourse",
                table: "Training_CourseCandidateCurriculumModification");

            migrationBuilder.DropIndex(
                name: "IX_Training_CourseCandidateCurriculumModification_CourseIdCourse",
                table: "Training_CourseCandidateCurriculumModification");

            migrationBuilder.DropColumn(
                name: "CourseIdCourse",
                table: "Training_CourseCandidateCurriculumModification");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseCandidateCurriculumModification_IdCourse",
                table: "Training_CourseCandidateCurriculumModification",
                column: "IdCourse");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_CourseCandidateCurriculumModification_Training_Course_IdCourse",
                table: "Training_CourseCandidateCurriculumModification",
                column: "IdCourse",
                principalTable: "Training_Course",
                principalColumn: "IdCourse",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_CourseCandidateCurriculumModification_Training_Course_IdCourse",
                table: "Training_CourseCandidateCurriculumModification");

            migrationBuilder.DropIndex(
                name: "IX_Training_CourseCandidateCurriculumModification_IdCourse",
                table: "Training_CourseCandidateCurriculumModification");

            migrationBuilder.AddColumn<int>(
                name: "CourseIdCourse",
                table: "Training_CourseCandidateCurriculumModification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseCandidateCurriculumModification_CourseIdCourse",
                table: "Training_CourseCandidateCurriculumModification",
                column: "CourseIdCourse");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_CourseCandidateCurriculumModification_Training_Course_CourseIdCourse",
                table: "Training_CourseCandidateCurriculumModification",
                column: "CourseIdCourse",
                principalTable: "Training_Course",
                principalColumn: "IdCourse",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
