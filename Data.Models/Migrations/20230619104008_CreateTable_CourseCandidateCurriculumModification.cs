using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_CourseCandidateCurriculumModification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_CourseCandidateCurriculumModification",
                columns: table => new
                {
                    IdCourseCandidateCurriculumModification = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с курс за обучение, предлаган от ЦПО"),
                    CourseIdCourse = table.Column<int>(type: "int", nullable: false),
                    IdCandidateCurriculumModification = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Програмa за обучение, предлагани от ЦПО"),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseCandidateCurriculumModification", x => x.IdCourseCandidateCurriculumModification);
                    table.ForeignKey(
                        name: "FK_Training_CourseCandidateCurriculumModification_Candidate_CurriculumModification_IdCandidateCurriculumModification",
                        column: x => x.IdCandidateCurriculumModification,
                        principalTable: "Candidate_CurriculumModification",
                        principalColumn: "IdCandidateCurriculumModification",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_CourseCandidateCurriculumModification_Training_Course_CourseIdCourse",
                        column: x => x.CourseIdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Връзка между курс и промяна на учебен план");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseCandidateCurriculumModification_CourseIdCourse",
                table: "Training_CourseCandidateCurriculumModification",
                column: "CourseIdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseCandidateCurriculumModification_IdCandidateCurriculumModification",
                table: "Training_CourseCandidateCurriculumModification",
                column: "IdCandidateCurriculumModification");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseCandidateCurriculumModification");
        }
    }
}
