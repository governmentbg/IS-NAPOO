using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_CurriculumSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_CurriculumSchedule",
                columns: table => new
                {
                    IdCurriculumSchedule = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTrainingCurriculum = table.Column<int>(type: "int", nullable: false),
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: true),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: true),
                    IdTrainingScheduleType = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CurriculumSchedule");
        }
    }
}
