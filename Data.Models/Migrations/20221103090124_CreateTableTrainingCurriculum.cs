using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableTrainingCurriculum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_Curriculum",
                columns: table => new
                {
                    IdTrainingCurriculum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateCurriculum = table.Column<int>(type: "int", nullable: false),
                    IdCandidateProviderSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdProgram = table.Column<int>(type: "int", nullable: false),
                    IdCourse = table.Column<int>(type: "int", nullable: true),
                    IdProfessionalTraining = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Theory = table.Column<double>(type: "float", nullable: true),
                    Practice = table.Column<double>(type: "float", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_Curriculum", x => x.IdTrainingCurriculum);
                    table.ForeignKey(
                        name: "FK_Training_Curriculum_Candidate_Curriculum_IdCandidateCurriculum",
                        column: x => x.IdCandidateCurriculum,
                        principalTable: "Candidate_Curriculum",
                        principalColumn: "IdCandidateCurriculum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Training_Curriculum_Candidate_ProviderSpeciality_IdCandidateProviderSpeciality",
                        column: x => x.IdCandidateProviderSpeciality,
                        principalTable: "Candidate_ProviderSpeciality",
                        principalColumn: "IdCandidateProviderSpeciality",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Training_Curriculum_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse");
                    table.ForeignKey(
                        name: "FK_Training_Curriculum_Training_Program_IdProgram",
                        column: x => x.IdProgram,
                        principalTable: "Training_Program",
                        principalColumn: "IdProgram",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Training_Curriculum_IdCandidateCurriculum",
                table: "Training_Curriculum",
                column: "IdCandidateCurriculum");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Curriculum_IdCandidateProviderSpeciality",
                table: "Training_Curriculum",
                column: "IdCandidateProviderSpeciality");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Curriculum_IdCourse",
                table: "Training_Curriculum",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Curriculum_IdProgram",
                table: "Training_Curriculum",
                column: "IdProgram");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_Curriculum");
        }
    }
}
