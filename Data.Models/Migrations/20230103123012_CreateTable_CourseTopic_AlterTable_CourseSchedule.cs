using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_CourseTopic_AlterTable_CourseSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CurriculumSchedule");

            migrationBuilder.CreateTable(
                name: "Training_CourseTopic",
                columns: table => new
                {
                    IdCourseTopic = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с курс за обучение, предлагани от ЦПО"),
                    Topic = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PracticeHours = table.Column<double>(type: "float", nullable: false, comment: "Сумарен брой часове по практика"),
                    TheoryHours = table.Column<double>(type: "float", nullable: false, comment: "Сумарен брой часове по теория"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Training_CourseSchedule",
                columns: table => new
                {
                    IdCourseSchedule = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourseTopic = table.Column<int>(type: "int", nullable: false),
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: true),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: true),
                    IdTrainingScheduleType = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseSchedule", x => x.IdCourseSchedule);
                    table.ForeignKey(
                        name: "FK_Training_CourseSchedule_Candidate_ProviderPremises_IdCandidateProviderPremises",
                        column: x => x.IdCandidateProviderPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises");
                    table.ForeignKey(
                        name: "FK_Training_CourseSchedule_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                        column: x => x.IdCandidateProviderTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer");
                    table.ForeignKey(
                        name: "FK_Training_CourseSchedule_Training_CourseTopic_IdCourseTopic",
                        column: x => x.IdCourseTopic,
                        principalTable: "Training_CourseTopic",
                        principalColumn: "IdCourseTopic",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseSchedule_IdCandidateProviderPremises",
                table: "Training_CourseSchedule",
                column: "IdCandidateProviderPremises");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseSchedule_IdCandidateProviderTrainer",
                table: "Training_CourseSchedule",
                column: "IdCandidateProviderTrainer");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseSchedule_IdCourseTopic",
                table: "Training_CourseSchedule",
                column: "IdCourseTopic");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseTopic_IdCourse",
                table: "Training_CourseTopic",
                column: "IdCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseSchedule");

            migrationBuilder.DropTable(
                name: "Training_CourseTopic");

            migrationBuilder.CreateTable(
                name: "Training_CurriculumSchedule",
                columns: table => new
                {
                    IdCurriculumSchedule = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: true),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: true),
                    IdTrainingCurriculum = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    IdTrainingScheduleType = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CurriculumSchedule", x => x.IdCurriculumSchedule);
                    table.ForeignKey(
                        name: "FK_Training_CurriculumSchedule_Candidate_ProviderPremises_IdCandidateProviderPremises",
                        column: x => x.IdCandidateProviderPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises");
                    table.ForeignKey(
                        name: "FK_Training_CurriculumSchedule_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                        column: x => x.IdCandidateProviderTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer");
                    table.ForeignKey(
                        name: "FK_Training_CurriculumSchedule_Training_Curriculum_IdTrainingCurriculum",
                        column: x => x.IdTrainingCurriculum,
                        principalTable: "Training_Curriculum",
                        principalColumn: "IdTrainingCurriculum",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Training_CurriculumSchedule_IdCandidateProviderPremises",
                table: "Training_CurriculumSchedule",
                column: "IdCandidateProviderPremises");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CurriculumSchedule_IdCandidateProviderTrainer",
                table: "Training_CurriculumSchedule",
                column: "IdCandidateProviderTrainer");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CurriculumSchedule_IdTrainingCurriculum",
                table: "Training_CurriculumSchedule",
                column: "IdTrainingCurriculum");
        }
    }
}
